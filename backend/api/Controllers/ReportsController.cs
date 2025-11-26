using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using app.Application.Reports.Queries.GetUsersReport;
using app.Application.Reports.Queries.GetAuditLogsReport;
using app.Application.Reports.Queries.GetFilesReport;
using app.Application.Reports.Queries.GetNotificationsReport;
using app.Application.Reports.Services;

namespace app.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPdfExportService _pdfExportService;
    private readonly IExcelExportService _excelExportService;

    public ReportsController(
        IMediator mediator,
        IPdfExportService pdfExportService,
        IExcelExportService excelExportService)
    {
        _mediator = mediator;
        _pdfExportService = pdfExportService;
        _excelExportService = excelExportService;
    }

    // ==================== USERS ====================

    [HttpGet("users")]
    public async Task<IActionResult> GetUsersReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetUsersReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        return Ok(report);
    }

    [HttpGet("users/pdf")]
    public async Task<IActionResult> GetUsersReportPdf([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetUsersReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        var pdfBytes = _pdfExportService.GenerateUsersReportPdf(report);
        
        return File(pdfBytes, "application/pdf", $"relatorio-usuarios-{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    [HttpGet("users/excel")]
    public async Task<IActionResult> GetUsersReportExcel([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetUsersReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        var excelBytes = _excelExportService.GenerateUsersReportExcel(report);
        
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"relatorio-usuarios-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    // ==================== AUDIT LOGS ====================

    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogsReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetAuditLogsReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        return Ok(report);
    }

    [HttpGet("audit-logs/pdf")]
    public async Task<IActionResult> GetAuditLogsReportPdf([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetAuditLogsReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        var pdfBytes = _pdfExportService.GenerateAuditLogsReportPdf(report);
        
        return File(pdfBytes, "application/pdf", $"relatorio-auditoria-{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    [HttpGet("audit-logs/excel")]
    public async Task<IActionResult> GetAuditLogsReportExcel([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetAuditLogsReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        var excelBytes = _excelExportService.GenerateAuditLogsReportExcel(report);
        
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"relatorio-auditoria-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    // ==================== FILES ====================

    [HttpGet("files")]
    public async Task<IActionResult> GetFilesReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetFilesReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        return Ok(report);
    }

    [HttpGet("files/pdf")]
    public async Task<IActionResult> GetFilesReportPdf([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetFilesReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        var pdfBytes = _pdfExportService.GenerateFilesReportPdf(report);
        
        return File(pdfBytes, "application/pdf", $"relatorio-arquivos-{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    [HttpGet("files/excel")]
    public async Task<IActionResult> GetFilesReportExcel([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetFilesReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        var excelBytes = _excelExportService.GenerateFilesReportExcel(report);
        
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"relatorio-arquivos-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    // ==================== NOTIFICATIONS ====================

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotificationsReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetNotificationsReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        return Ok(report);
    }

    [HttpGet("notifications/pdf")]
    public async Task<IActionResult> GetNotificationsReportPdf([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetNotificationsReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        var pdfBytes = _pdfExportService.GenerateNotificationsReportPdf(report);
        
        return File(pdfBytes, "application/pdf", $"relatorio-notificacoes-{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }

    [HttpGet("notifications/excel")]
    public async Task<IActionResult> GetNotificationsReportExcel([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetNotificationsReportQuery { StartDate = startDate, EndDate = endDate };
        var report = await _mediator.Send(query);
        var excelBytes = _excelExportService.GenerateNotificationsReportExcel(report);
        
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"relatorio-notificacoes-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}
