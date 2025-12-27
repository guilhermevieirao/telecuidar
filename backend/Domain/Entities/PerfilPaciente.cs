using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Perfil específico para usuários do tipo Paciente
/// Relacionamento 1:1 com Usuario
/// </summary>
public class PerfilPaciente : EntidadeBase
{
    // Referência ao usuário
    public Guid UsuarioId { get; set; }
    
    // Dados de identificação
    public string? Cns { get; set; } // Cartão Nacional de Saúde
    public string? NomeSocial { get; set; }
    
    // Dados pessoais
    public string? Sexo { get; set; } // M, F, Outro
    public DateTime? DataNascimento { get; set; }
    public string? NomeMae { get; set; }
    public string? NomePai { get; set; }
    public string? Nacionalidade { get; set; }
    
    // Endereço
    public string? Cep { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; } // UF
    
    // Navigation Property
    public Usuario Usuario { get; set; } = null!;
}
