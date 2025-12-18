using Application.DTOs.Appointments;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedAppointmentsDto>> GetAppointments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var result = await _appointmentService.GetAppointmentsAsync(page, pageSize, search, status, startDate, endDate);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointment(Guid id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
        if (appointment == null)
            return NotFound();

        return Ok(appointment);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var appointment = await _appointmentService.CreateAppointmentAsync(dto);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<AppointmentDto>> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentDto dto)
    {
        var appointment = await _appointmentService.UpdateAppointmentAsync(id, dto);
        if (appointment == null)
            return NotFound();

        return Ok(appointment);
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelAppointment(Guid id)
    {
        var result = await _appointmentService.CancelAppointmentAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "Appointment cancelled successfully" });
    }

    [HttpPost("{id}/finish")]
    public async Task<ActionResult> FinishAppointment(Guid id)
    {
        var result = await _appointmentService.FinishAppointmentAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "Appointment finished successfully" });
    }
}
