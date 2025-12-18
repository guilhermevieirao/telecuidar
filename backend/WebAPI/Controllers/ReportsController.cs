using Application.DTOs.Reports;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats([FromQuery] Guid? userId = null)
    {
        var stats = await _reportService.GetDashboardStatsAsync(userId);
        return Ok(stats);
    }

    [HttpGet("appointments")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<ReportDto>> GenerateAppointmentsReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var report = await _reportService.GenerateAppointmentsReportAsync(startDate, endDate);
        return Ok(report);
    }

    [HttpGet("specialties")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<List<SpecialtyStatsDto>>> GetSpecialtyStats()
    {
        var stats = await _reportService.GetSpecialtyStatsAsync();
        return Ok(stats);
    }
}
