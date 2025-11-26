using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Security.Claims;
using app.Application.Notifications.Commands.CreateNotification;
using app.Application.Notifications.Commands.MarkAsRead;
using app.Application.Notifications.Queries.GetMyNotifications;
using app.Application.Notifications.Queries.GetUnreadCount;

namespace app.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] bool? onlyUnread, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetMyNotificationsQuery
        {
            UserId = GetCurrentUserId(),
            OnlyUnread = onlyUnread,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var query = new GetUnreadCountQuery
        {
            UserId = GetCurrentUserId()
        };

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationCommand command)
    {
        command.CreatedByUserId = GetCurrentUserId();
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPatch("{id}/mark-as-read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var command = new MarkNotificationAsReadCommand
        {
            NotificationId = id,
            UserId = GetCurrentUserId()
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}
