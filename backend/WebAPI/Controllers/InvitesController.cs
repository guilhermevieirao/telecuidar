using Application.DTOs.Invites;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvitesController : ControllerBase
{
    private readonly IInviteService _inviteService;

    public InvitesController(IInviteService inviteService)
    {
        _inviteService = inviteService;
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<PaginatedInvitesDto>> GetInvites(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? role = null,
        [FromQuery] string? status = null)
    {
        try
        {
            var result = await _inviteService.GetInvitesAsync(page, pageSize, sortBy, sortDirection, role, status);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<InviteDto>> GetInvite(Guid id)
    {
        try
        {
            var invite = await _inviteService.GetInviteByIdAsync(id);
            if (invite == null)
            {
                return NotFound(new { message = "Invite not found" });
            }
            return Ok(invite);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<InviteDto>> CreateInvite([FromBody] CreateInviteDto dto)
    {
        try
        {
            var invite = await _inviteService.CreateInviteAsync(dto);
            return CreatedAtAction(nameof(GetInvite), new { id = invite.Id }, invite);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteInvite(Guid id)
    {
        try
        {
            var result = await _inviteService.DeleteInviteAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Invite not found" });
            }
            return Ok(new { message = "Invite deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
}
