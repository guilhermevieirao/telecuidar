using Application.DTOs.Consultas;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;

/// <summary>
/// Controller de consultas
/// </summary>
[ApiController]
[Route("api/consultas")]
[Authorize]
public class ConsultasController : ControllerBase
{
    private readonly IConsultaService _consultaService;
    private readonly ILogAuditoriaService _logAuditoriaService;
    private readonly INotificacaoService _notificacaoService;

    public ConsultasController(
        IConsultaService consultaService, 
        ILogAuditoriaService logAuditoriaService,
        INotificacaoService notificacaoService)
    {
        _consultaService = consultaService;
        _logAuditoriaService = logAuditoriaService;
        _notificacaoService = notificacaoService;
    }

    /// <summary>
    /// Listar consultas com filtros
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ConsultasPaginadasDto>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        [FromQuery] string? busca = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null,
        [FromQuery] Guid? pacienteId = null,
        [FromQuery] Guid? profissionalId = null)
    {
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        // Pacientes só veem suas próprias consultas
        if (tipo == "Paciente")
        {
            pacienteId = usuarioId;
        }
        // Profissionais veem as consultas deles
        else if (tipo == "Profissional")
        {
            profissionalId = usuarioId;
        }

        var resultado = await _consultaService.ObterConsultasAsync(pagina, tamanhoPagina, busca, status, dataInicio, dataFim, pacienteId, profissionalId);
        return Ok(resultado);
    }

    /// <summary>
    /// Obter consulta por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ConsultaDto>> ObterPorId(Guid id)
    {
        var consulta = await _consultaService.ObterConsultaPorIdAsync(id);
        if (consulta == null)
        {
            return NotFound(new { mensagem = "Consulta não encontrada." });
        }

        // Verificar permissão
        var usuarioId = ObterUsuarioIdAtual();
        var tipo = ObterTipoUsuario();

        if (tipo == "Paciente" && consulta.PacienteId != usuarioId)
        {
            return Forbid();
        }

        if (tipo == "Profissional" && consulta.ProfissionalId != usuarioId)
        {
            return Forbid();
        }

        return Ok(consulta);
    }

    /// <summary>
    /// Agendar nova consulta
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ConsultaDto>> Agendar([FromBody] CriarConsultaDto dto)
    {
        try
        {
            var usuarioId = ObterUsuarioIdAtual();
            var tipo = ObterTipoUsuario();

            // Se for paciente, usar seu próprio ID
            if (tipo == "Paciente")
            {
                dto.PacienteId = usuarioId;
            }

            var consulta = await _consultaService.CriarConsultaAsync(dto);

            await _logAuditoriaService.RegistrarAsync(
                usuarioId,
                "agendar",
                "Consulta",
                consulta.Id,
                null,
                dto,
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            // Enviar notificações
            await _notificacaoService.EnviarNotificacaoNovaConsultaAsync(
                dto.PacienteId, 
                dto.ProfissionalId, 
                DateTime.Parse(dto.Data),
                dto.Horario);

            return CreatedAtAction(nameof(ObterPorId), new { id = consulta.Id }, consulta);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar consulta
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ConsultaDto>> Atualizar(Guid id, [FromBody] AtualizarConsultaDto dto)
    {
        try
        {
            var consulta = await _consultaService.AtualizarConsultaAsync(id, dto);
            if (consulta == null)
            {
                return NotFound(new { mensagem = "Consulta não encontrada." });
            }

            await _logAuditoriaService.RegistrarAsync(
                ObterUsuarioIdAtual(),
                "atualizar",
                "Consulta",
                id,
                null,
                dto,
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return Ok(consulta);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Cancelar consulta
    /// </summary>
    [HttpPost("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        try
        {
            var sucesso = await _consultaService.CancelarConsultaAsync(id);
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Consulta não encontrada." });
            }

            await _logAuditoriaService.RegistrarAsync(
                ObterUsuarioIdAtual(),
                "cancelar",
                "Consulta",
                id,
                null,
                null,
                ObterEnderecoIp(),
                ObterUserAgent()
            );

            return Ok(new { mensagem = "Consulta cancelada com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Iniciar consulta (profissional)
    /// </summary>
    [HttpPost("{id:guid}/iniciar")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<IActionResult> Iniciar(Guid id)
    {
        try
        {
            var sucesso = await _consultaService.IniciarConsultaAsync(id);
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Consulta não encontrada." });
            }

            return Ok(new { mensagem = "Consulta iniciada com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Finalizar consulta (profissional)
    /// </summary>
    [HttpPost("{id:guid}/finalizar")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<IActionResult> Finalizar(Guid id)
    {
        try
        {
            var sucesso = await _consultaService.FinalizarConsultaAsync(id);
            if (!sucesso)
            {
                return NotFound(new { mensagem = "Consulta não encontrada." });
            }

            return Ok(new { mensagem = "Consulta finalizada com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Verificar horários disponíveis
    /// </summary>
    [HttpGet("horarios-disponiveis")]
    public async Task<ActionResult<List<string>>> ObterHorariosDisponiveis(
        [FromQuery] Guid profissionalId,
        [FromQuery] string data)
    {
        if (!DateTime.TryParse(data, out var dataConsulta))
        {
            return BadRequest(new { mensagem = "Data inválida." });
        }

        var horarios = await _consultaService.ObterHorariosDisponiveisAsync(profissionalId, dataConsulta);
        return Ok(horarios);
    }

    // ========== PRÉ-CONSULTA ==========

    /// <summary>
    /// Obter pré-consulta
    /// </summary>
    [HttpGet("{id:guid}/pre-consulta")]
    public async Task<ActionResult<PreConsultaDto>> ObterPreConsulta(Guid id)
    {
        var preConsulta = await _consultaService.ObterPreConsultaAsync(id);
        if (preConsulta == null)
        {
            return NotFound(new { mensagem = "Pré-consulta não encontrada." });
        }

        return Ok(preConsulta);
    }

    /// <summary>
    /// Salvar pré-consulta (paciente)
    /// </summary>
    [HttpPost("{id:guid}/pre-consulta")]
    public async Task<ActionResult<PreConsultaDto>> SalvarPreConsulta(Guid id, [FromBody] PreConsultaDto dto)
    {
        try
        {
            dto.ConsultaId = id;
            var preConsulta = await _consultaService.SalvarPreConsultaAsync(id, dto);
            return Ok(preConsulta);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // ========== ANAMNESE ==========

    /// <summary>
    /// Obter anamnese
    /// </summary>
    [HttpGet("{id:guid}/anamnese")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<AnamneseDto>> ObterAnamnese(Guid id)
    {
        var anamnese = await _consultaService.ObterAnamneseAsync(id);
        if (anamnese == null)
        {
            return NotFound(new { mensagem = "Anamnese não encontrada." });
        }

        return Ok(anamnese);
    }

    /// <summary>
    /// Salvar anamnese (profissional)
    /// </summary>
    [HttpPost("{id:guid}/anamnese")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<AnamneseDto>> SalvarAnamnese(Guid id, [FromBody] AnamneseDto dto)
    {
        try
        {
            dto.ConsultaId = id;
            var anamnese = await _consultaService.SalvarAnamneseAsync(id, dto);
            return Ok(anamnese);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // ========== SOAP ==========

    /// <summary>
    /// Obter registro SOAP
    /// </summary>
    [HttpGet("{id:guid}/soap")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<RegistroSoapDto>> ObterSoap(Guid id)
    {
        var soap = await _consultaService.ObterSoapAsync(id);
        if (soap == null)
        {
            return NotFound(new { mensagem = "Registro SOAP não encontrado." });
        }

        return Ok(soap);
    }

    /// <summary>
    /// Salvar registro SOAP (profissional)
    /// </summary>
    [HttpPost("{id:guid}/soap")]
    [Authorize(Roles = "Profissional,Administrador")]
    public async Task<ActionResult<RegistroSoapDto>> SalvarSoap(Guid id, [FromBody] RegistroSoapDto dto)
    {
        try
        {
            dto.ConsultaId = id;
            var soap = await _consultaService.SalvarSoapAsync(id, dto);
            return Ok(soap);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // ========== DADOS BIOMÉTRICOS ==========

    /// <summary>
    /// Obter dados biométricos
    /// </summary>
    [HttpGet("{id:guid}/biometricos")]
    public async Task<ActionResult<DadosBiometricosDto>> ObterDadosBiometricos(Guid id)
    {
        var dados = await _consultaService.ObterDadosBiometricosAsync(id);
        if (dados == null)
        {
            return NotFound(new { mensagem = "Dados biométricos não encontrados." });
        }

        return Ok(dados);
    }

    /// <summary>
    /// Salvar dados biométricos
    /// </summary>
    [HttpPost("{id:guid}/biometricos")]
    public async Task<ActionResult<DadosBiometricosDto>> SalvarDadosBiometricos(Guid id, [FromBody] DadosBiometricosDto dto)
    {
        try
        {
            dto.ConsultaId = id;
            var dados = await _consultaService.SalvarDadosBiometricosAsync(id, dto);
            return Ok(dados);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    private Guid ObterUsuarioIdAtual()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;
        
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private string ObterTipoUsuario()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "Paciente";
    }

    private string? ObterEnderecoIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? ObterUserAgent()
    {
        return HttpContext.Request.Headers.UserAgent.FirstOrDefault();
    }
}
