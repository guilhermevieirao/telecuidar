using MediatR;
using Microsoft.AspNetCore.Mvc;
using app.Application.Users.Queries.GetAllUsers;

namespace app.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
}