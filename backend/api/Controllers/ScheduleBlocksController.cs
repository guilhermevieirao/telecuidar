using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using app.Application.ScheduleBlocks.Commands;
using app.Application.ScheduleBlocks.Queries;
using app.Application.Common.Models;
using System.Security.Claims;

namespace app.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ScheduleBlocksController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScheduleBlocksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Solicitar bloqueio (Profissional)
    [HttpPost]
    [Authorize(Roles = "Profissional")]
    public async Task<IActionResult> RequestBlock([FromBody] RequestScheduleBlockCommand command)
    {
        try
        {
            Console.WriteLine($"[ScheduleBlocksController] RequestBlock chamado: {System.Text.Json.JsonSerializer.Serialize(command)}");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                command.ProfessionalId = userId;
            }
            else
            {
                Console.WriteLine("[ScheduleBlocksController] Não foi possível obter o ProfessionalId do usuário autenticado.");
            }
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                Console.WriteLine($"[ScheduleBlocksController] Falha ao solicitar bloqueio: {result.Message} | {string.Join(",", result.Errors ?? new List<string>())}");
                return BadRequest(new { result.Message, result.Errors });
            }
            Console.WriteLine("[ScheduleBlocksController] Bloqueio solicitado com sucesso.");
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ScheduleBlocksController] Erro inesperado: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    // Listar bloqueios do profissional
    [HttpGet("my")]
    [Authorize(Roles = "Profissional")]
    public async Task<IActionResult> GetMyBlocks()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();
        var result = await _mediator.Send(new GetProfessionalScheduleBlocksQuery { ProfessionalId = userId });
        return Ok(result.Data);
    }

    // Listar bloqueios para o admin
    [HttpGet]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetAllBlocks()
    {
        var result = await _mediator.Send(new GetAllScheduleBlocksQuery());
        return Ok(result.Data);
    }

    // Aceitar bloqueio
    [HttpPost("{id}/accept")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> AcceptBlock(int id, [FromBody] AdminRespondScheduleBlockCommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            command.AdminId = userId;
        }
        command.BlockId = id;
        command.Accept = true;
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });
        return Ok(result.Data);
    }

    // Recusar bloqueio
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RejectBlock(int id, [FromBody] AdminRespondScheduleBlockCommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            command.AdminId = userId;
        }
        command.BlockId = id;
        command.Accept = false;
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });
        return Ok(result.Data);
    }
}
