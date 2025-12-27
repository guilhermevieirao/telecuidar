using Application.DTOs.AI;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de integração com IA (DeepSeek)
/// </summary>
public class IAService : IIAService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IAService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _modelo;

    public IAService(IConfiguration configuration, ILogger<IAService> logger, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("DeepSeek");
        _apiKey = _configuration["DeepSeek:ApiKey"] ?? "";
        _baseUrl = _configuration["DeepSeek:BaseUrl"] ?? "https://api.deepseek.com/v1";
        _modelo = _configuration["DeepSeek:Model"] ?? "deepseek-chat";

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<RespostaIADto> GerarResumoConsultaAsync(DadosConsultaIADto dados)
    {
        var prompt = ConstruirPromptResumo(dados);
        return await EnviarMensagemAsync(prompt, "resumo_consulta");
    }

    public async Task<RespostaIADto> SugerirDiagnosticoAsync(DadosConsultaIADto dados)
    {
        var prompt = ConstruirPromptDiagnostico(dados);
        return await EnviarMensagemAsync(prompt, "sugestao_diagnostico");
    }

    public async Task<RespostaIADto> SugerirCondutaAsync(DadosConsultaIADto dados)
    {
        var prompt = ConstruirPromptConduta(dados);
        return await EnviarMensagemAsync(prompt, "sugestao_conduta");
    }

    public async Task<RespostaIADto> GerarTextoAtestadoAsync(string tipo, int dias, string? condicao)
    {
        var prompt = $@"Gere um texto profissional para atestado médico do tipo '{tipo}' com as seguintes características:
- Período: {dias} dias
- Condição médica (se aplicável): {condicao ?? "não especificada"}

O texto deve ser formal, objetivo e seguir as normas brasileiras de documentação médica.
NÃO inclua dados do paciente ou médico, apenas o corpo do texto.";

        return await EnviarMensagemAsync(prompt, "texto_atestado");
    }

    public async Task<RespostaIADto> AnalisarSintomasAsync(List<string> sintomas, DadosBiometricosIADto? biometricos)
    {
        var prompt = new System.Text.StringBuilder();
        prompt.AppendLine("Analise os seguintes sintomas reportados pelo paciente e forneça informações educativas:");
        prompt.AppendLine($"\nSintomas: {string.Join(", ", sintomas)}");

        if (biometricos != null)
        {
            prompt.AppendLine("\nDados biométricos:");
            if (biometricos.PressaoSistolica.HasValue)
                prompt.AppendLine($"- Pressão arterial: {biometricos.PressaoSistolica}/{biometricos.PressaoDiastolica} mmHg");
            if (biometricos.FrequenciaCardiaca.HasValue)
                prompt.AppendLine($"- Frequência cardíaca: {biometricos.FrequenciaCardiaca} bpm");
            if (biometricos.Temperatura.HasValue)
                prompt.AppendLine($"- Temperatura: {biometricos.Temperatura}°C");
            if (biometricos.SaturacaoOxigenio.HasValue)
                prompt.AppendLine($"- Saturação O2: {biometricos.SaturacaoOxigenio}%");
            if (biometricos.GlicemiaCapilar.HasValue)
                prompt.AppendLine($"- Glicemia capilar: {biometricos.GlicemiaCapilar} mg/dL");
        }

        prompt.AppendLine("\nFORNEÇA:");
        prompt.AppendLine("1. Possíveis condições relacionadas (apenas informativo, não diagnóstico)");
        prompt.AppendLine("2. Sinais de alerta que requerem atendimento imediato");
        prompt.AppendLine("3. Cuidados gerais recomendados");
        prompt.AppendLine("\nIMPORTANTE: Inclua disclaimer que isto não substitui avaliação médica profissional.");

        return await EnviarMensagemAsync(prompt.ToString(), "analise_sintomas");
    }

    public async Task<RespostaIADto> ProcessarMensagemAsync(string mensagem, string contexto)
    {
        var prompt = $@"Contexto da conversa: {contexto}

Mensagem do usuário: {mensagem}

Responda de forma profissional, empática e dentro do escopo de telemedicina.
Se a pergunta for sobre diagnóstico específico, oriente a consultar um profissional.";

        return await EnviarMensagemAsync(prompt, "chat");
    }

    private async Task<RespostaIADto> EnviarMensagemAsync(string prompt, string tipo)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogWarning("API Key do DeepSeek não configurada");
            return new RespostaIADto
            {
                Sucesso = false,
                Erro = "Serviço de IA não configurado.",
                Tipo = tipo
            };
        }

        try
        {
            var request = new
            {
                model = _modelo,
                messages = new[]
                {
                    new { role = "system", content = "Você é um assistente médico virtual para uma plataforma de telemedicina brasileira. Responda sempre em português brasileiro, de forma profissional e ética. Nunca forneça diagnósticos definitivos, sempre recomende consulta com profissional." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 2000
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/chat/completions", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Erro na API DeepSeek: {StatusCode} - {Error}", response.StatusCode, error);
                return new RespostaIADto
                {
                    Sucesso = false,
                    Erro = "Erro ao processar solicitação de IA.",
                    Tipo = tipo
                };
            }

            var resultado = await response.Content.ReadFromJsonAsync<DeepSeekResponse>();

            return new RespostaIADto
            {
                Sucesso = true,
                Conteudo = resultado?.choices?.FirstOrDefault()?.message?.content ?? "",
                Tipo = tipo,
                TokensUtilizados = resultado?.usage?.total_tokens ?? 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao chamar API DeepSeek");
            return new RespostaIADto
            {
                Sucesso = false,
                Erro = "Erro interno ao processar IA.",
                Tipo = tipo
            };
        }
    }

    private static string ConstruirPromptResumo(DadosConsultaIADto dados)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Gere um resumo clínico estruturado da seguinte consulta:");
        sb.AppendLine();

        if (!string.IsNullOrEmpty(dados.QueixaPrincipal))
            sb.AppendLine($"QUEIXA PRINCIPAL: {dados.QueixaPrincipal}");

        if (!string.IsNullOrEmpty(dados.HistoriaDoencaAtual))
            sb.AppendLine($"\nHISTÓRIA DA DOENÇA ATUAL: {dados.HistoriaDoencaAtual}");

        if (dados.Sintomas?.Any() == true)
            sb.AppendLine($"\nSINTOMAS: {string.Join(", ", dados.Sintomas)}");

        if (!string.IsNullOrEmpty(dados.ExameFisico))
            sb.AppendLine($"\nEXAME FÍSICO: {dados.ExameFisico}");

        if (!string.IsNullOrEmpty(dados.Soap?.Subjetivo))
            sb.AppendLine($"\nS (Subjetivo): {dados.Soap.Subjetivo}");
        if (!string.IsNullOrEmpty(dados.Soap?.Objetivo))
            sb.AppendLine($"O (Objetivo): {dados.Soap.Objetivo}");
        if (!string.IsNullOrEmpty(dados.Soap?.Avaliacao))
            sb.AppendLine($"A (Avaliação): {dados.Soap.Avaliacao}");
        if (!string.IsNullOrEmpty(dados.Soap?.Plano))
            sb.AppendLine($"P (Plano): {dados.Soap.Plano}");

        sb.AppendLine("\nGere um resumo conciso e profissional desta consulta.");

        return sb.ToString();
    }

    private static string ConstruirPromptDiagnostico(DadosConsultaIADto dados)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Com base nos dados clínicos abaixo, sugira hipóteses diagnósticas (em ordem de probabilidade):");
        sb.AppendLine();

        if (!string.IsNullOrEmpty(dados.QueixaPrincipal))
            sb.AppendLine($"QUEIXA PRINCIPAL: {dados.QueixaPrincipal}");

        if (dados.Sintomas?.Any() == true)
            sb.AppendLine($"SINTOMAS: {string.Join(", ", dados.Sintomas)}");

        if (!string.IsNullOrEmpty(dados.HistoriaDoencaAtual))
            sb.AppendLine($"HDA: {dados.HistoriaDoencaAtual}");

        if (!string.IsNullOrEmpty(dados.ExameFisico))
            sb.AppendLine($"EXAME FÍSICO: {dados.ExameFisico}");

        if (dados.Biometricos != null)
        {
            sb.AppendLine("SINAIS VITAIS:");
            if (dados.Biometricos.PressaoSistolica.HasValue)
                sb.AppendLine($"  PA: {dados.Biometricos.PressaoSistolica}/{dados.Biometricos.PressaoDiastolica} mmHg");
            if (dados.Biometricos.FrequenciaCardiaca.HasValue)
                sb.AppendLine($"  FC: {dados.Biometricos.FrequenciaCardiaca} bpm");
            if (dados.Biometricos.Temperatura.HasValue)
                sb.AppendLine($"  Temp: {dados.Biometricos.Temperatura}°C");
        }

        sb.AppendLine("\nForneça 3-5 hipóteses diagnósticas com CID-10 correspondente, se possível.");
        sb.AppendLine("IMPORTANTE: Estas são apenas sugestões para auxiliar o médico, não diagnósticos definitivos.");

        return sb.ToString();
    }

    private static string ConstruirPromptConduta(DadosConsultaIADto dados)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Sugira uma conduta clínica baseada nos seguintes dados:");
        sb.AppendLine();

        if (!string.IsNullOrEmpty(dados.QueixaPrincipal))
            sb.AppendLine($"QUEIXA: {dados.QueixaPrincipal}");

        if (!string.IsNullOrEmpty(dados.Soap?.Avaliacao))
            sb.AppendLine($"AVALIAÇÃO: {dados.Soap.Avaliacao}");

        if (dados.Sintomas?.Any() == true)
            sb.AppendLine($"SINTOMAS: {string.Join(", ", dados.Sintomas)}");

        sb.AppendLine("\nSugira:");
        sb.AppendLine("1. Exames complementares (se necessário)");
        sb.AppendLine("2. Medicações (genéricos, com doses comuns no Brasil)");
        sb.AppendLine("3. Orientações ao paciente");
        sb.AppendLine("4. Necessidade de encaminhamento a especialista");
        sb.AppendLine("5. Retorno ou acompanhamento");
        sb.AppendLine("\nIMPORTANTE: Sugestões baseadas em protocolos clínicos brasileiros. Decisão final é do médico.");

        return sb.ToString();
    }

    // Classes para deserialização da resposta
    private class DeepSeekResponse
    {
        public List<Choice>? choices { get; set; }
        public Usage? usage { get; set; }
    }

    private class Choice
    {
        public Message? message { get; set; }
    }

    private class Message
    {
        public string? content { get; set; }
    }

    private class Usage
    {
        public int total_tokens { get; set; }
    }
}
