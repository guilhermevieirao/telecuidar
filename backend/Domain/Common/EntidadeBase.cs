namespace Domain.Common;

/// <summary>
/// Classe base para todas as entidades do domínio
/// </summary>
public abstract class EntidadeBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}
