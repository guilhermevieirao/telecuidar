using ClosedXML.Excel;
using app.Application.Reports.DTOs;

namespace app.Application.Reports.Services;

public interface IExcelExportService
{
    byte[] GenerateUsersReportExcel(UsersReportDto report);
    byte[] GenerateAuditLogsReportExcel(AuditLogsReportDto report);
    byte[] GenerateFilesReportExcel(FilesReportDto report);
    byte[] GenerateNotificationsReportExcel(NotificationsReportDto report);
}

public class ExcelExportService : IExcelExportService
{
    public byte[] GenerateUsersReportExcel(UsersReportDto report)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Usuários");

        // Título e cabeçalho
        worksheet.Cell("A1").Value = "TeleCuidar - Relatório de Usuários";
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.LightBlue;
        worksheet.Range("A1:F1").Merge();

        worksheet.Cell("A2").Value = $"Gerado em: {report.GeneratedAt:dd/MM/yyyy HH:mm}";
        worksheet.Range("A2:F2").Merge();

        // Resumo
        worksheet.Cell("A4").Value = "Resumo";
        worksheet.Cell("A4").Style.Font.Bold = true;
        worksheet.Cell("A4").Style.Fill.BackgroundColor = XLColor.LightGray;
        worksheet.Range("A4:B4").Merge();

        worksheet.Cell("A5").Value = "Total de Usuários:";
        worksheet.Cell("B5").Value = report.TotalUsers;
        worksheet.Cell("A6").Value = "Ativos:";
        worksheet.Cell("B6").Value = report.ActiveUsers;
        worksheet.Cell("A7").Value = "Inativos:";
        worksheet.Cell("B7").Value = report.InactiveUsers;
        worksheet.Cell("A8").Value = "E-mails Confirmados:";
        worksheet.Cell("B8").Value = report.EmailConfirmedCount;
        worksheet.Cell("A9").Value = "Pacientes:";
        worksheet.Cell("B9").Value = report.PacientesCount;
        worksheet.Cell("A10").Value = "Profissionais:";
        worksheet.Cell("B10").Value = report.ProfissionaisCount;
        worksheet.Cell("A11").Value = "Administradores:";
        worksheet.Cell("B11").Value = report.AdministradoresCount;

        // Cabeçalhos da tabela de detalhes
        int startRow = 13;
        worksheet.Cell(startRow, 1).Value = "ID";
        worksheet.Cell(startRow, 2).Value = "Nome Completo";
        worksheet.Cell(startRow, 3).Value = "E-mail";
        worksheet.Cell(startRow, 4).Value = "Tipo";
        worksheet.Cell(startRow, 5).Value = "Ativo";
        worksheet.Cell(startRow, 6).Value = "E-mail Confirmado";
        worksheet.Cell(startRow, 7).Value = "Data de Cadastro";
        worksheet.Cell(startRow, 8).Value = "Último Acesso";

        var headerRange = worksheet.Range(startRow, 1, startRow, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        // Dados
        int row = startRow + 1;
        foreach (var user in report.UserDetails)
        {
            worksheet.Cell(row, 1).Value = user.Id;
            worksheet.Cell(row, 2).Value = user.FullName;
            worksheet.Cell(row, 3).Value = user.Email;
            worksheet.Cell(row, 4).Value = user.Role;
            worksheet.Cell(row, 5).Value = user.IsActive ? "Sim" : "Não";
            worksheet.Cell(row, 6).Value = user.EmailConfirmed ? "Sim" : "Não";
            worksheet.Cell(row, 7).Value = user.CreatedAt.ToString("dd/MM/yyyy");
            worksheet.Cell(row, 8).Value = user.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
            
            if (!user.IsActive)
            {
                worksheet.Range(row, 1, row, 8).Style.Font.FontColor = XLColor.Red;
            }
            
            row++;
        }

        // Ajustar largura das colunas
        worksheet.Columns().AdjustToContents();
        
        // Adicionar filtros
        var dataRange = worksheet.Range(startRow, 1, row - 1, 8);
        dataRange.SetAutoFilter();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateAuditLogsReportExcel(AuditLogsReportDto report)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Audit Logs");

        // Título
        worksheet.Cell("A1").Value = "TeleCuidar - Relatório de Auditoria";
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.Orange;
        worksheet.Range("A1:G1").Merge();

        worksheet.Cell("A2").Value = $"Período: {report.StartDate:dd/MM/yyyy} até {report.EndDate:dd/MM/yyyy}";
        worksheet.Range("A2:G2").Merge();

        // Resumo
        worksheet.Cell("A4").Value = "Resumo";
        worksheet.Cell("A4").Style.Font.Bold = true;
        worksheet.Cell("A4").Style.Fill.BackgroundColor = XLColor.LightGray;
        worksheet.Range("A4:B4").Merge();

        worksheet.Cell("A5").Value = "Total de Logs:";
        worksheet.Cell("B5").Value = report.TotalLogs;

        int summaryRow = 7;
        worksheet.Cell(summaryRow, 1).Value = "Ações Mais Comuns";
        worksheet.Cell(summaryRow, 1).Style.Font.Bold = true;
        worksheet.Cell(summaryRow, 3).Value = "Entidades Mais Modificadas";
        worksheet.Cell(summaryRow, 3).Style.Font.Bold = true;

        summaryRow++;
        foreach (var action in report.ActionCounts.OrderByDescending(x => x.Value).Take(10))
        {
            worksheet.Cell(summaryRow, 1).Value = action.Key;
            worksheet.Cell(summaryRow, 2).Value = action.Value;
            summaryRow++;
        }

        summaryRow = 8;
        foreach (var entity in report.EntityCounts.OrderByDescending(x => x.Value).Take(10))
        {
            worksheet.Cell(summaryRow, 3).Value = entity.Key;
            worksheet.Cell(summaryRow, 4).Value = entity.Value;
            summaryRow++;
        }

        // Cabeçalhos da tabela de detalhes
        int startRow = Math.Max(summaryRow, 18) + 2;
        worksheet.Cell(startRow, 1).Value = "ID";
        worksheet.Cell(startRow, 2).Value = "Usuário";
        worksheet.Cell(startRow, 3).Value = "Ação";
        worksheet.Cell(startRow, 4).Value = "Entidade";
        worksheet.Cell(startRow, 5).Value = "ID Entidade";
        worksheet.Cell(startRow, 6).Value = "Endereço IP";
        worksheet.Cell(startRow, 7).Value = "Data/Hora";

        var headerRange = worksheet.Range(startRow, 1, startRow, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        // Dados
        int row = startRow + 1;
        foreach (var log in report.LogDetails)
        {
            worksheet.Cell(row, 1).Value = log.Id;
            worksheet.Cell(row, 2).Value = log.UserName;
            worksheet.Cell(row, 3).Value = log.Action;
            worksheet.Cell(row, 4).Value = log.EntityName;
            worksheet.Cell(row, 5).Value = log.EntityId?.ToString() ?? "-";
            worksheet.Cell(row, 6).Value = log.IpAddress ?? "-";
            worksheet.Cell(row, 7).Value = log.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
            row++;
        }

        worksheet.Columns().AdjustToContents();
        var dataRange = worksheet.Range(startRow, 1, row - 1, 7);
        dataRange.SetAutoFilter();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateFilesReportExcel(FilesReportDto report)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Arquivos");

        // Título
        worksheet.Cell("A1").Value = "TeleCuidar - Relatório de Arquivos";
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.Purple;
        worksheet.Cell("A1").Style.Font.FontColor = XLColor.White;
        worksheet.Range("A1:F1").Merge();

        worksheet.Cell("A2").Value = $"Período: {report.StartDate:dd/MM/yyyy} até {report.EndDate:dd/MM/yyyy}";
        worksheet.Range("A2:F2").Merge();

        // Resumo
        worksheet.Cell("A4").Value = "Resumo";
        worksheet.Cell("A4").Style.Font.Bold = true;
        worksheet.Cell("A4").Style.Fill.BackgroundColor = XLColor.LightGray;
        worksheet.Range("A4:B4").Merge();

        worksheet.Cell("A5").Value = "Total de Arquivos:";
        worksheet.Cell("B5").Value = report.TotalFiles;
        worksheet.Cell("A6").Value = "Tamanho Total:";
        worksheet.Cell("B6").Value = report.TotalSizeFormatted;

        int summaryRow = 8;
        worksheet.Cell(summaryRow, 1).Value = "Por Categoria";
        worksheet.Cell(summaryRow, 1).Style.Font.Bold = true;
        worksheet.Cell(summaryRow, 3).Value = "Uploads por Usuário";
        worksheet.Cell(summaryRow, 3).Style.Font.Bold = true;

        summaryRow++;
        foreach (var category in report.CategoryCounts)
        {
            worksheet.Cell(summaryRow, 1).Value = category.Key;
            worksheet.Cell(summaryRow, 2).Value = category.Value;
            summaryRow++;
        }

        summaryRow = 9;
        foreach (var user in report.UserUploadCounts.OrderByDescending(x => x.Value))
        {
            worksheet.Cell(summaryRow, 3).Value = user.Key;
            worksheet.Cell(summaryRow, 4).Value = user.Value;
            summaryRow++;
        }

        // Cabeçalhos da tabela de detalhes
        int startRow = Math.Max(summaryRow, 15) + 2;
        worksheet.Cell(startRow, 1).Value = "ID";
        worksheet.Cell(startRow, 2).Value = "Nome do Arquivo";
        worksheet.Cell(startRow, 3).Value = "Categoria";
        worksheet.Cell(startRow, 4).Value = "Tamanho";
        worksheet.Cell(startRow, 5).Value = "Enviado Por";
        worksheet.Cell(startRow, 6).Value = "Data de Upload";

        var headerRange = worksheet.Range(startRow, 1, startRow, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        // Dados
        int row = startRow + 1;
        foreach (var file in report.FileDetails)
        {
            worksheet.Cell(row, 1).Value = file.Id;
            worksheet.Cell(row, 2).Value = file.OriginalFileName;
            worksheet.Cell(row, 3).Value = file.FileCategory;
            worksheet.Cell(row, 4).Value = file.FileSizeFormatted;
            worksheet.Cell(row, 5).Value = file.UploadedByUserName;
            worksheet.Cell(row, 6).Value = file.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            row++;
        }

        worksheet.Columns().AdjustToContents();
        var dataRange = worksheet.Range(startRow, 1, row - 1, 6);
        dataRange.SetAutoFilter();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateNotificationsReportExcel(NotificationsReportDto report)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Notificações");

        // Título
        worksheet.Cell("A1").Value = "TeleCuidar - Relatório de Notificações";
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.Green;
        worksheet.Cell("A1").Style.Font.FontColor = XLColor.White;
        worksheet.Range("A1:F1").Merge();

        worksheet.Cell("A2").Value = $"Período: {report.StartDate:dd/MM/yyyy} até {report.EndDate:dd/MM/yyyy}";
        worksheet.Range("A2:F2").Merge();

        // Resumo
        worksheet.Cell("A4").Value = "Resumo";
        worksheet.Cell("A4").Style.Font.Bold = true;
        worksheet.Cell("A4").Style.Fill.BackgroundColor = XLColor.LightGray;
        worksheet.Range("A4:B4").Merge();

        worksheet.Cell("A5").Value = "Total de Notificações:";
        worksheet.Cell("B5").Value = report.TotalNotifications;
        worksheet.Cell("A6").Value = "Lidas:";
        worksheet.Cell("B6").Value = report.ReadNotifications;
        worksheet.Cell("A7").Value = "Não Lidas:";
        worksheet.Cell("B7").Value = report.UnreadNotifications;
        worksheet.Cell("A8").Value = "Taxa de Leitura:";
        worksheet.Cell("B8").Value = $"{report.ReadPercentage:F1}%";

        int summaryRow = 10;
        worksheet.Cell(summaryRow, 1).Value = "Por Tipo";
        worksheet.Cell(summaryRow, 1).Style.Font.Bold = true;
        
        summaryRow++;
        foreach (var type in report.TypeCounts)
        {
            worksheet.Cell(summaryRow, 1).Value = type.Key;
            worksheet.Cell(summaryRow, 2).Value = type.Value;
            summaryRow++;
        }

        // Cabeçalhos da tabela de detalhes
        int startRow = summaryRow + 2;
        worksheet.Cell(startRow, 1).Value = "ID";
        worksheet.Cell(startRow, 2).Value = "Título";
        worksheet.Cell(startRow, 3).Value = "Tipo";
        worksheet.Cell(startRow, 4).Value = "Usuário";
        worksheet.Cell(startRow, 5).Value = "Status";
        worksheet.Cell(startRow, 6).Value = "Data de Criação";

        var headerRange = worksheet.Range(startRow, 1, startRow, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        // Dados
        int row = startRow + 1;
        foreach (var notif in report.NotificationDetails)
        {
            worksheet.Cell(row, 1).Value = notif.Id;
            worksheet.Cell(row, 2).Value = notif.Title;
            worksheet.Cell(row, 3).Value = notif.Type;
            worksheet.Cell(row, 4).Value = notif.UserName;
            worksheet.Cell(row, 5).Value = notif.IsRead ? "Lida" : "Não Lida";
            worksheet.Cell(row, 6).Value = notif.CreatedAt.ToString("dd/MM/yyyy HH:mm");
            
            if (notif.IsRead)
            {
                worksheet.Cell(row, 5).Style.Font.FontColor = XLColor.Green;
            }
            else
            {
                worksheet.Cell(row, 5).Style.Font.FontColor = XLColor.Red;
            }
            
            row++;
        }

        worksheet.Columns().AdjustToContents();
        var dataRange = worksheet.Range(startRow, 1, row - 1, 6);
        dataRange.SetAutoFilter();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
