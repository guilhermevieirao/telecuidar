using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using app.Application.Specialties.Commands;
using app.Application.Specialties.Queries;
using app.Application.Specialties.DTOs;

namespace app.Api.Controllers;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class SpecialtiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SpecialtiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var query = new GetAllSpecialtiesQuery { IncludeInactive = includeInactive };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("{id}/professionals")]
    public async Task<IActionResult> GetProfessionals(int id)
    {
        var query = new GetProfessionalsBySpecialtyQuery { SpecialtyId = id };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(new { isSuccess = true, data = result.Data });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserSpecialties(int userId)
    {
        var query = new GetUserSpecialtiesQuery { UserId = userId };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(new { isSuccess = true, data = result.Data });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSpecialtyCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return CreatedAtAction(nameof(GetAll), new { id = result.Data }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSpecialtyCommand command)
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
        var command = new DeleteSpecialtyCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return NoContent();
    }

    [HttpPost("{specialtyId}/professionals/{userId}")]
    public async Task<IActionResult> AssignToProfessional(int specialtyId, int userId)
    {
        var command = new AssignSpecialtyToProfessionalCommand
        {
            SpecialtyId = specialtyId,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok();
    }

    [HttpDelete("{specialtyId}/professionals/{userId}")]
    public async Task<IActionResult> RemoveFromProfessional(int specialtyId, int userId)
    {
        var command = new RemoveSpecialtyFromProfessionalCommand
        {
            SpecialtyId = specialtyId,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return NoContent();
    }

    // Endpoints para campos personalizados
    [HttpGet("{specialtyId}/fields")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFields(int specialtyId)
    {
        var query = new GetSpecialtyFieldsQuery(specialtyId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{specialtyId}/fields")]
    public async Task<IActionResult> CreateField(int specialtyId, [FromBody] CreateSpecialtyFieldDto fieldDto)
    {
        var command = new CreateSpecialtyFieldCommand(specialtyId, fieldDto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetFields), new { specialtyId }, result);
    }

    [HttpPut("fields/{fieldId}")]
    public async Task<IActionResult> UpdateField(int fieldId, [FromBody] UpdateSpecialtyFieldDto fieldDto)
    {
        var command = new UpdateSpecialtyFieldCommand(fieldId, fieldDto);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("fields/{fieldId}")]
    public async Task<IActionResult> DeleteField(int fieldId)
    {
        var command = new DeleteSpecialtyFieldCommand(fieldId);
        await _mediator.Send(command);
        return NoContent();
    }
}
