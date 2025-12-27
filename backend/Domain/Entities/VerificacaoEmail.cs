using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Token de verificação de email (para criação de conta ou troca de email)
/// </summary>
public class VerificacaoEmail : EntidadeBase
{
    public Guid UsuarioId { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // criacao_conta, troca_email
    public DateTime ExpiraEm { get; set; }
    public bool Utilizado { get; set; } = false;
    public DateTime? UtilizadoEm { get; set; }
    
    // Navigation Property
    public Usuario Usuario { get; set; } = null!;
}
