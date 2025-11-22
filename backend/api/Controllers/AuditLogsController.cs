using MediatR;
using Microsoft.AspNetCore.Mvc;
using app.Application.AuditLogs.Queries.GetAuditLogs;

namespace app.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtém logs de auditoria
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? userId,
        [FromQuery] string? action,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var query = new GetAuditLogsQuery
        {
            UserId = userId,
            Action = action,
            StartDate = startDate,
            EndDate = endDate
        };
        
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
}
