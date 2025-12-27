namespace Application.DTOs.Notificacoes;

/// <summary>
/// DTO de notificação para retorno
/// </summary>
public class NotificacaoDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string Tipo { get; set; } = "info";
    public bool Lida { get; set; }
    public string? Link { get; set; }
    public DateTime CriadoEm { get; set; }
}

/// <summary>
/// DTO para criar notificação
/// </summary>
public class CriarNotificacaoDto
{
    public Guid UsuarioId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string? Tipo { get; set; }
    public string? Link { get; set; }
}

/// <summary>
/// DTO para listagem paginada de notificações
/// </summary>
public class NotificacoesPaginadasDto
{
    public List<NotificacaoDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int NaoLidas { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}
