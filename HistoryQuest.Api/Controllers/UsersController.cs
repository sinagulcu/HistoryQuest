using HistoryQuest.Application.Auth.UseCases;
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

    [HttpDelete("{id.guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _deleteUserCommand.ExecuteAsync(id);
        return NoContent();
    }
}
