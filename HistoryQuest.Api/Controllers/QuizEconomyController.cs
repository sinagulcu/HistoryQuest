using HistoryQuest.Application.Credits.DTOs;
using HistoryQuest.Application.Credits.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/Quiz/{quizId:guid}/economy")]
[Authorize(Roles = "Teacher,Admin")]
public class QuizEconomyController : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> SetRule(Guid quizId,
        [FromBody] SetQuizEconomyRuleRequest request,
        [FromServices] SetQuizEconomyRuleCommand command,
        CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim)) return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        var isAdmin = User.IsInRole("Admin");

        await command.ExecuteAsync(quizId, userId, isAdmin, request, ct);
        return Ok();
    }
}
