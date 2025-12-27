using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Certificado digital salvo pelo profissional para assinaturas
/// Relacionamento N:1 com Usuario (Profissional)
/// </summary>
public class CertificadoSalvo : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public Guid ProfissionalId { get; set; }
    
    // Alias para compatibilidade
    public Guid UsuarioId { get => ProfissionalId; set => ProfissionalId = value; }
    
    public string NomeSujeito { get; set; } = string.Empty;
    public string NomeEmissor { get; set; } = string.Empty;
    public DateTime ValidoDe { get; set; }
    public DateTime ValidoAte { get; set; }
    public string ImpressaoDigital { get; set; } = string.Empty;
    public string DadosPfxCriptografados { get; set; } = string.Empty;
    public bool RequerSenhaAoUsar { get; set; }
    public string? SenhaCriptografada { get; set; }
    public string? CaminhoArquivo { get; set; }
    
    // Aliases para compatibilidade com serviços
    public string Apelido { get => Nome; set => Nome = value; }
    public string? NomeProprietario { get => NomeSujeito; set => NomeSujeito = value ?? string.Empty; }
    public string? CpfCnpj { get; set; }
    public DateTime DataValidade { get => ValidoAte; set => ValidoAte = value; }
    public string? Thumbprint { get => ImpressaoDigital; set => ImpressaoDigital = value ?? string.Empty; }
    public string? HashSenha { get => SenhaCriptografada; set => SenhaCriptografada = value; }
    
    // Navigation Property
    public Usuario Profissional { get; set; } = null!;
}
