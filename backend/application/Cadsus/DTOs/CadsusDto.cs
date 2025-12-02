namespace app.Application.Cadsus.DTOs;

public class CadsusTokenStatus
{
    public bool HasToken { get; set; }
    public bool IsValid { get; set; }
    public string ExpiresAt { get; set; } = "";
    public string ExpiresIn { get; set; } = "";
    public long ExpiresInMs { get; set; }
    public string Message { get; set; } = "";
}

public class CadsusCidadao
{
    // Identificação Principal
    public string Cns { get; set; } = "";
    public string Cpf { get; set; } = "";
    public string Nome { get; set; } = "";
    public string DataNascimento { get; set; } = "";
    public string StatusCadastro { get; set; } = "";
    
    // Filiação
    public string NomeMae { get; set; } = "";
    public string NomePai { get; set; } = "";
    
    // Características
    public string Sexo { get; set; } = "";
    public string RacaCor { get; set; } = "";
    
    // Endereço
    public string TipoLogradouro { get; set; } = "";
    public string Logradouro { get; set; } = "";
    public string Numero { get; set; } = "";
    public string Complemento { get; set; } = "";
    public string Cidade { get; set; } = "";
    public string CodigoCidade { get; set; } = "";
    public string PaisEnderecoAtual { get; set; } = "";
    public string Cep { get; set; } = "";
    public string EnderecoCompleto { get; set; } = "";
    
    // Naturalidade
    public string CidadeNascimento { get; set; } = "";
    public string CodigoCidadeNascimento { get; set; } = "";
    public string PaisNascimento { get; set; } = "";
    public string CodigoPaisNascimento { get; set; } = "";
    
    // Contato
    public List<string> Telefones { get; set; } = new();
    public List<string> Emails { get; set; } = new();
}
