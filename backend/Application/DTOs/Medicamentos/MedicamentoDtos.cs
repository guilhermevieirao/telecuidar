namespace Application.DTOs.Medicamentos;

/// <summary>
/// DTO de medicamento ANVISA para retorno
/// </summary>
public class MedicamentoAnvisaDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? NomeComercial { get; set; }
    public string? Principio { get; set; }
    public string? PrincipioAtivo { get; set; }
    public string? Laboratorio { get; set; }
    public string? Apresentacao { get; set; }
    public string? Classe { get; set; }
    public string? NumeroRegistro { get; set; }
}

/// <summary>
/// DTO para buscar medicamentos
/// </summary>
public class BuscarMedicamentosDto
{
    public string Termo { get; set; } = string.Empty;
    public int Limite { get; set; } = 20;
}

/// <summary>
/// DTO para resposta de busca de medicamentos
/// </summary>
public class BuscarMedicamentosResponseDto
{
    public List<MedicamentoAnvisaDto> Dados { get; set; } = new();
    public int Total { get; set; }
}

/// <summary>
/// DTO para listagem paginada de medicamentos
/// </summary>
public class MedicamentosPaginadosDto
{
    public List<MedicamentoAnvisaDto> Dados { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas { get; set; }
}
