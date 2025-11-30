using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using app.Application.Schedules.Commands;
using app.Application.Schedules.Queries;
using System.Security.Claims;

namespace app.Api.Controllers;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SchedulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? professionalId, [FromQuery] bool? isActive)
    {
        var query = new GetAllSchedulesQuery 
        { 
            ProfessionalId = professionalId,
            IsActive = isActive
        };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleCommand command)
    {
        // Capturar o ID do usuário autenticado
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            command.CreatedByUserId = userId;
        }

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return CreatedAtAction(nameof(GetAll), new { id = result.Data }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScheduleCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteScheduleCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return NoContent();
    }
}
