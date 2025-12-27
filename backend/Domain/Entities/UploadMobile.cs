using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Upload temporário feito via dispositivo móvel
/// Para sistemas de pré-consulta e chat de anexos
/// </summary>
public class UploadMobile : EntidadeBase
{
    public string Token { get; set; } = string.Empty;
    public Guid? UsuarioId { get; set; }
    public Guid? ConsultaId { get; set; }
    
    public string Origem { get; set; } = string.Empty; // pre-consulta, chat-anexos
    public string NomeArquivo { get; set; } = string.Empty;
    public string CaminhoArquivo { get; set; } = string.Empty;
    public string TipoArquivo { get; set; } = string.Empty;
    public long TamanhoArquivo { get; set; }
    
    public bool Processado { get; set; } = false;
    public DateTime ExpiraEm { get; set; }
    
    // Navigation Properties
    public Usuario? Usuario { get; set; }
    public Consulta? Consulta { get; set; }
}
