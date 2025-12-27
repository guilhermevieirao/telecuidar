using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Notificação para usuário
/// Relacionamento N:1 com Usuario
/// </summary>
public class Notificacao : EntidadeBase
{
    public Guid UsuarioId { get; set; }
    
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string Tipo { get; set; } = "info"; // info, warning, error, success
    public bool Lida { get; set; } = false;
    public string? Link { get; set; }
    
    // Navigation Property
    public Usuario Usuario { get; set; } = null!;
}
