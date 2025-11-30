using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using app.Application.Appointments.Commands;
using app.Application.Appointments.Queries;

namespace app.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Buscar especialidades com agenda disponível
    /// </summary>
    [HttpGet("available-specialties")]
    public async Task<IActionResult> GetAvailableSpecialties()
    {
        var query = new GetAvailableSpecialtiesQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }

    /// <summary>
    /// Buscar datas disponíveis para uma especialidade
    /// </summary>
    [HttpGet("available-dates")]
    public async Task<IActionResult> GetAvailableDates([FromQuery] int specialtyId, [FromQuery] int daysAhead = 30)
    {
        var query = new GetAvailableDatesQuery
        {
            SpecialtyId = specialtyId,
            DaysAhead = daysAhead
        };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }

    /// <summary>
    /// Buscar horários disponíveis para uma especialidade em uma data específica
    /// </summary>
    [HttpGet("available-time-slots")]
    public async Task<IActionResult> GetAvailableTimeSlots([FromQuery] int specialtyId, [FromQuery] DateTime date)
    {
        var query = new GetAvailableTimeSlotsQuery
        {
            SpecialtyId = specialtyId,
            Date = date
        };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }

    /// <summary>
    /// Criar um novo agendamento
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentCommand command)
    {
        // Garantir que o paciente só pode agendar para si mesmo
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        command.PatientId = userId;

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return CreatedAtAction(nameof(GetPatientAppointments), new { }, result.Data);
    }

    /// <summary>
    /// Buscar agendamentos do paciente
    /// </summary>
    [HttpGet("my-appointments")]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> GetPatientAppointments([FromQuery] bool includePast = false)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var query = new GetPatientAppointmentsQuery
        {
            PatientId = userId,
            IncludePast = includePast
        };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }

    /// <summary>
    /// Buscar agendamentos do profissional
    /// </summary>
    [HttpGet("my-professional-appointments")]
    [Authorize(Roles = "Profissional")]
    public async Task<IActionResult> GetProfessionalAppointments([FromQuery] bool includePast = false)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var query = new GetProfessionalAppointmentsQuery
        {
            ProfessionalId = userId,
            IncludePast = includePast
        };
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }

    /// <summary>
    /// Cancelar um agendamento (Paciente)
    /// </summary>
    [HttpPut("{id}/cancel")]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> CancelAppointment(int id, [FromBody] CancelAppointmentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var command = new CancelAppointmentCommand
        {
            AppointmentId = id,
            PatientId = userId,
            CancellationReason = request.Reason
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok();
    }

    /// <summary>
    /// Cancelar um agendamento (Profissional)
    /// </summary>
    [HttpPut("{id}/cancel-professional")]
    [Authorize(Roles = "Profissional")]
    public async Task<IActionResult> CancelAppointmentByProfessional(int id, [FromBody] CancelAppointmentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var command = new CancelAppointmentCommand
        {
            AppointmentId = id,
            ProfessionalId = userId,
            CancellationReason = request.Reason
        };

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { result.Message, result.Errors });

        return Ok();
    }

    /// <summary>
    /// DEBUG: Buscar todos os agendamentos (temporário)
    /// </summary>
    [HttpGet("debug/all")]
    public async Task<IActionResult> GetAllAppointmentsDebug()
    {
        var query = new GetPatientAppointmentsQuery
        {
            PatientId = 0, // Vai ser ignorado
            IncludePast = true
        };
        
        // Vou criar uma query especial para debug
        return Ok("Endpoint de debug - implementar se necessário");
    }
}

public class CancelAppointmentRequest
{
    public string Reason { get; set; } = string.Empty;
}
