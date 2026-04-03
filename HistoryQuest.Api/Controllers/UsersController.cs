using HistoryQuest.Application.Auth.UseCases;
using HistoryQuest.Application.Users.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly DeleteUserCommand _deleteUserCommand;

    public UsersController(DeleteUserCommand deleteUserCommand)
    {
        _deleteUserCommand = deleteUserCommand;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromServices] GetUsersQuery query)
    {
        var data = await query.ExecuteAsync();
        return Ok(data);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("count")]
    public async Task<IActionResult> GetCount([FromServices] GetUserCountQuery query)
    {
        var count = await query.ExecuteAsync();
        return Ok(new { totalUsers = count });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id.guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _deleteUserCommand.ExecuteAsync(id);
        return NoContent();
    }
}
