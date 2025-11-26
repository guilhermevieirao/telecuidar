using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using app.Application.Reports.DTOs;

namespace app.Application.Reports.Services;

public interface IPdfExportService
{
    byte[] GenerateUsersReportPdf(UsersReportDto report);
    byte[] GenerateAuditLogsReportPdf(AuditLogsReportDto report);
    byte[] GenerateFilesReportPdf(FilesReportDto report);
    byte[] GenerateNotificationsReportPdf(NotificationsReportDto report);
}

public class PdfExportService : IPdfExportService
{
    public PdfExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateUsersReportPdf(UsersReportDto report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Height(80).Background(Colors.Blue.Lighten3).Padding(10).Column(column =>
                {
                    column.Item().Text("TeleCuidar").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text("Relatório de Usuários").FontSize(16).FontColor(Colors.Grey.Darken2);
                    column.Item().Text($"Gerado em: {report.GeneratedAt:dd/MM/yyyy HH:mm}").FontSize(9).FontColor(Colors.Grey.Medium);
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    column.Spacing(10);

                    // Resumo
                    column.Item().Background(Colors.Blue.Lighten4).Padding(15).Column(summary =>
                    {
                        summary.Item().Text("Resumo Geral").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                        summary.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Total de Usuários: {report.TotalUsers}").Bold();
                                col.Item().Text($"Ativos: {report.ActiveUsers}");
                                col.Item().Text($"Inativos: {report.InactiveUsers}");
                            });
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"E-mails Confirmados: {report.EmailConfirmedCount}").Bold();
                                col.Item().Text($"Pacientes: {report.PacientesCount}");
                                col.Item().Text($"Profissionais: {report.ProfissionaisCount}");
                                col.Item().Text($"Administradores: {report.AdministradoresCount}");
                            });
                        });
                    });

                    // Tabela de detalhes
                    column.Item().PaddingTop(20).Text("Detalhes dos Usuários").FontSize(12).Bold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // ID
                            columns.RelativeColumn(3); // Nome
                            columns.RelativeColumn(3); // Email
                            columns.RelativeColumn(2); // Role
                            columns.ConstantColumn(50); // Status
                            columns.RelativeColumn(2); // Cadastro
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("ID").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Nome").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("E-mail").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tipo").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Status").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Cadastro").Bold();
                        });

                        // Rows
                        foreach (var user in report.UserDetails.Take(50)) // Limit for PDF size
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(user.Id.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(user.FullName);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(user.Email).FontSize(8);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(user.Role);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(user.IsActive ? "✓ Ativo" : "✗ Inativo").FontColor(user.IsActive ? Colors.Green.Medium : Colors.Red.Medium);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(user.CreatedAt.ToString("dd/MM/yyyy"));
                        }
                    });

                    if (report.UserDetails.Count > 50)
                    {
                        column.Item().PaddingTop(10).Text($"* Mostrando 50 de {report.UserDetails.Count} usuários").FontSize(9).Italic().FontColor(Colors.Grey.Medium);
                    }
                });

                page.Footer().Height(30).Background(Colors.Grey.Lighten3).Padding(10).AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateAuditLogsReportPdf(AuditLogsReportDto report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Height(80).Background(Colors.Orange.Lighten3).Padding(10).Column(column =>
                {
                    column.Item().Text("TeleCuidar").FontSize(24).Bold().FontColor(Colors.Orange.Darken2);
                    column.Item().Text("Relatório de Auditoria").FontSize(16).FontColor(Colors.Grey.Darken2);
                    column.Item().Text($"Período: {report.StartDate:dd/MM/yyyy} até {report.EndDate:dd/MM/yyyy}").FontSize(9).FontColor(Colors.Grey.Medium);
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    column.Spacing(10);

                    // Resumo
                    column.Item().Background(Colors.Orange.Lighten4).Padding(15).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Total de Logs: {report.TotalLogs}").Bold();
                            col.Item().PaddingTop(5).Text("Ações mais comuns:").FontSize(9);
                            foreach (var action in report.ActionCounts.OrderByDescending(x => x.Value).Take(5))
                            {
                                col.Item().Text($"• {action.Key}: {action.Value}").FontSize(8);
                            }
                        });
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Entidades mais modificadas:").FontSize(9).Bold();
                            foreach (var entity in report.EntityCounts.OrderByDescending(x => x.Value).Take(5))
                            {
                                col.Item().Text($"• {entity.Key}: {entity.Value}").FontSize(8);
                            }
                        });
                    });

                    // Tabela de detalhes
                    column.Item().PaddingTop(20).Text("Últimos Logs").FontSize(12).Bold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // ID
                            columns.RelativeColumn(2); // Usuário
                            columns.RelativeColumn(2); // Ação
                            columns.RelativeColumn(2); // Entidade
                            columns.ConstantColumn(40); // EntityId
                            columns.RelativeColumn(2); // IP
                            columns.RelativeColumn(2); // Data
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("ID").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Usuário").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Ação").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Entidade").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("ID Ent.").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("IP").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Data/Hora").Bold();
                        });

                        // Rows
                        foreach (var log in report.LogDetails.Take(100))
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.Id.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.UserName).FontSize(8);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.Action).FontSize(8);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.EntityName).FontSize(8);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.EntityId?.ToString() ?? "-");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.IpAddress ?? "-").FontSize(7);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(log.CreatedAt.ToString("dd/MM/yy HH:mm")).FontSize(8);
                        }
                    });
                });

                page.Footer().Height(30).Background(Colors.Grey.Lighten3).Padding(10).AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateFilesReportPdf(FilesReportDto report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Height(80).Background(Colors.Purple.Lighten3).Padding(10).Column(column =>
                {
                    column.Item().Text("TeleCuidar").FontSize(24).Bold().FontColor(Colors.Purple.Darken2);
                    column.Item().Text("Relatório de Arquivos").FontSize(16).FontColor(Colors.Grey.Darken2);
                    column.Item().Text($"Período: {report.StartDate:dd/MM/yyyy} até {report.EndDate:dd/MM/yyyy}").FontSize(9).FontColor(Colors.Grey.Medium);
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    column.Spacing(10);

                    // Resumo
                    column.Item().Background(Colors.Purple.Lighten4).Padding(15).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Total de Arquivos: {report.TotalFiles}").Bold();
                            col.Item().Text($"Tamanho Total: {report.TotalSizeFormatted}").Bold().FontColor(Colors.Purple.Darken1);
                            col.Item().PaddingTop(5).Text("Por categoria:").FontSize(9);
                            foreach (var category in report.CategoryCounts)
                            {
                                col.Item().Text($"• {category.Key}: {category.Value}").FontSize(8);
                            }
                        });
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Uploads por usuário:").FontSize(9).Bold();
                            foreach (var user in report.UserUploadCounts.OrderByDescending(x => x.Value).Take(10))
                            {
                                col.Item().Text($"• {user.Key}: {user.Value}").FontSize(8);
                            }
                        });
                    });

                    // Tabela de detalhes
                    column.Item().PaddingTop(20).Text("Arquivos Recentes").FontSize(12).Bold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("ID").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Nome do Arquivo").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Categoria").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tamanho").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Enviado Por").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Data").Bold();
                        });

                        foreach (var file in report.FileDetails.Take(50))
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(file.Id.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(file.OriginalFileName).FontSize(8);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(file.FileCategory).FontSize(8);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(file.FileSizeFormatted);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(file.UploadedByUserName).FontSize(8);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(file.CreatedAt.ToString("dd/MM/yyyy"));
                        }
                    });
                });

                page.Footer().Height(30).Background(Colors.Grey.Lighten3).Padding(10).AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateNotificationsReportPdf(NotificationsReportDto report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Height(80).Background(Colors.Green.Lighten3).Padding(10).Column(column =>
                {
                    column.Item().Text("TeleCuidar").FontSize(24).Bold().FontColor(Colors.Green.Darken2);
                    column.Item().Text("Relatório de Notificações").FontSize(16).FontColor(Colors.Grey.Darken2);
                    column.Item().Text($"Período: {report.StartDate:dd/MM/yyyy} até {report.EndDate:dd/MM/yyyy}").FontSize(9).FontColor(Colors.Grey.Medium);
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                {
                    column.Spacing(10);

                    // Resumo
                    column.Item().Background(Colors.Green.Lighten4).Padding(15).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text($"Total de Notificações: {report.TotalNotifications}").Bold();
                            col.Item().Text($"Lidas: {report.ReadNotifications}").FontColor(Colors.Green.Medium);
                            col.Item().Text($"Não Lidas: {report.UnreadNotifications}").FontColor(Colors.Red.Medium);
                            col.Item().Text($"Taxa de Leitura: {report.ReadPercentage:F1}%").Bold().FontColor(Colors.Blue.Medium);
                        });
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Por tipo:").FontSize(9).Bold();
                            foreach (var type in report.TypeCounts)
                            {
                                col.Item().Text($"• {type.Key}: {type.Value}").FontSize(8);
                            }
                        });
                    });

                    // Tabela de detalhes
                    column.Item().PaddingTop(20).Text("Notificações Recentes").FontSize(12).Bold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.ConstantColumn(60);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("ID").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Título").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tipo").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Usuário").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Status").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Data").Bold();
                        });

                        foreach (var notif in report.NotificationDetails.Take(50))
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(notif.Id.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(notif.Title).FontSize(8);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(notif.Type).FontSize(8);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(notif.UserName).FontSize(8);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(notif.IsRead ? "✓ Lida" : "✗ Não lida").FontColor(notif.IsRead ? Colors.Green.Medium : Colors.Red.Medium);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(notif.CreatedAt.ToString("dd/MM/yyyy"));
                        }
                    });
                });

                page.Footer().Height(30).Background(Colors.Grey.Lighten3).Padding(10).AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }
}
