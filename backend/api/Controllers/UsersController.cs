using MediatR;
using Microsoft.AspNetCore.Mvc;
using app.Application.Users.Commands.CreateUser;
using app.Application.Users.Commands.UpdateUser;
using app.Application.Users.Commands.UpdateUserByAdmin;
using app.Application.Users.Commands.DeleteUser;
using app.Application.Users.Queries.GetAllUsers;
using app.Application.Users.Queries.GetUserById;

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

    /// <summary>
    /// Obtém todos os usuários
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? role, [FromQuery] string? searchTerm)
    {
        var query = new GetAllUsersQuery 
        { 
            Role = role,
            SearchTerm = searchTerm
        };
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Obtém um usuário por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return NotFound(result);
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { message = "ID da URL não corresponde ao ID do corpo da requisição" });
        }

        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Atualiza um usuário (Admin)
    /// </summary>
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateByAdmin(Guid id, [FromBody] UpdateUserByAdminCommand command)
    {
        command.UserId = id;
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Deleta um usuário
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteUserCommand(id));
        
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }
}