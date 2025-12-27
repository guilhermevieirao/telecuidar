using Application.DTOs.AI;

namespace Application.Interfaces;

/// <summary>
/// Serviço de integração com IA (DeepSeek)
/// </summary>
public interface IIAService
{
    /// <summary>
    /// Gera resumo de uma consulta usando IA
    /// </summary>
    Task<RespostaIADto> GerarResumoConsultaAsync(DadosConsultaIADto dados);

    /// <summary>
    /// Sugere diagnóstico baseado nos dados da consulta
    /// </summary>
    Task<RespostaIADto> SugerirDiagnosticoAsync(DadosConsultaIADto dados);

    /// <summary>
    /// Sugere conduta clínica baseada nos dados
    /// </summary>
    Task<RespostaIADto> SugerirCondutaAsync(DadosConsultaIADto dados);

    /// <summary>
    /// Gera texto para atestado médico
    /// </summary>
    Task<RespostaIADto> GerarTextoAtestadoAsync(string tipo, int dias, string? condicao);

    /// <summary>
    /// Analisa sintomas e fornece informações educativas
    /// </summary>
    Task<RespostaIADto> AnalisarSintomasAsync(List<string> sintomas, DadosBiometricosIADto? biometricos);

    /// <summary>
    /// Processa uma mensagem no contexto de telemedicina
    /// </summary>
    Task<RespostaIADto> ProcessarMensagemAsync(string mensagem, string contexto);
}
