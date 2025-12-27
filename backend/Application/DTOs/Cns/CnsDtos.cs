namespace Application.DTOs.Cns;

/// <summary>
/// DTO para validar CNS
/// </summary>
public class ValidarCnsDto
{
    public string Cns { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de validação de CNS
/// </summary>
public class ValidarCnsResponseDto
{
    public bool Valido { get; set; }
    public string? Mensagem { get; set; }
    public string? TipoCns { get; set; }
}

/// <summary>
/// DTO para buscar informações por CNS
/// </summary>
public class BuscarInfoCnsDto
{
    public string Cns { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de informações do CNS
/// </summary>
public class InfoCnsResponseDto
{
    public bool Encontrado { get; set; }
    public string? Nome { get; set; }
    public string? DataNascimento { get; set; }
    public string? Sexo { get; set; }
    public string? NomeMae { get; set; }
    public string? Mensagem { get; set; }
}
