using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace app.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AIController> _logger;

    public AIController(
        IConfiguration configuration, 
        IHttpClientFactory httpClientFactory,
        ILogger<AIController> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeMedicalData([FromBody] JsonElement requestBody)
    {
        try
        {
            var apiKey = _configuration["DeepSeek:ApiKey"];
            var apiUrl = _configuration["DeepSeek:ApiUrl"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("DeepSeek API Key não configurada");
                return BadRequest(new { error = "API Key não configurada no servidor" });
            }

            if (string.IsNullOrEmpty(apiUrl))
            {
                apiUrl = "https://api.deepseek.com/v1/chat/completions";
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Enviando requisição para DeepSeek API");

            var response = await httpClient.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erro na API do DeepSeek: {response.StatusCode} - {responseContent}");
                return StatusCode((int)response.StatusCode, new { 
                    error = "Erro ao comunicar com a API de IA",
                    details = responseContent 
                });
            }

            _logger.LogInformation("Resposta recebida com sucesso da DeepSeek API");

            return Content(responseContent, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar requisição de análise de IA");
            return StatusCode(500, new { error = "Erro interno ao processar análise", message = ex.Message });
        }
    }
}
