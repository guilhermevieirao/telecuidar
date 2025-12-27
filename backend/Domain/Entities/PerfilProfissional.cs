using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Perfil específico para usuários do tipo Profissional
/// Relacionamento 1:1 com Usuario
/// </summary>
public class PerfilProfissional : EntidadeBase
{
    // Referência ao usuário
    public Guid UsuarioId { get; set; }
    
    // Dados profissionais
    public string? Crm { get; set; } // Conselho Regional de Medicina
    public string? Cbo { get; set; } // Classificação Brasileira de Ocupações
    public Guid? EspecialidadeId { get; set; }
    
    /// <summary>
    /// Tipo de registro profissional (CRM, CRO, COREN, etc.)
    /// </summary>
    public string? TipoRegistro { get; set; }
    
    /// <summary>
    /// Número do registro profissional
    /// </summary>
    public string? NumeroRegistro { get; set; }
    
    // Dados pessoais
    public string? Sexo { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Nacionalidade { get; set; }
    
    // Endereço
    public string? Cep { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    
    // Navigation Properties
    public Usuario Usuario { get; set; } = null!;
    public Especialidade? Especialidade { get; set; }
}
