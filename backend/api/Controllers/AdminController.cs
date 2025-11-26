using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using app.Application.Admin.Queries.GetUserStatistics;
using app.Application.Admin.Queries.GetAuditLogs;
using app.Application.Admin.Queries.GetAllInvitations;
using app.Application.Admin.Queries.GetInvitationByToken;
using app.Application.Admin.Commands.CreateInvitation;
using app.Application.Users.Commands.CreateUser;
using System.Security.Claims;

namespace app.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém estatísticas gerais do sistema
    /// </summary>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _mediator.Send(new GetUserStatisticsQuery());
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Obtém logs de auditoria com filtros opcionais
    /// </summary>
    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int? userId,
        [FromQuery] string? entityName,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] string? sortDirection = "desc")
    {
        var query = new GetAuditLogsQuery
        {
            UserId = userId,
            EntityName = entityName,
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDirection = sortDirection
        };

        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Cria um novo usuário (Admin pode criar qualquer role)
    /// </summary>
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(CreateUser), new { id = result.Data?.Id }, result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Cria um convite para registro de novo usuário
    /// </summary>
    [HttpPost("invitations")]
    public async Task<IActionResult> CreateInvitation([FromBody] CreateInvitationCommand command)
    {
        // Obter ID do usuário atual do token JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Usuário não autenticado" });
        }

        command.CreatedByUserId = userId;
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Lista todos os convites
    /// </summary>
    [HttpGet("invitations")]
    public async Task<IActionResult> GetInvitations()
    {
        var query = new GetAllInvitationsQuery();
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Obtém detalhes de um convite pelo token (endpoint público para registro)
    /// </summary>
    [HttpGet("invitations/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetInvitationByToken(string token)
    {
        var query = new GetInvitationByTokenQuery { Token = token };
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return NotFound(result);
    }
}
