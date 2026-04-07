using HistoryQuest.Application.Credits.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSummary([FromServices] GetWalletSummaryQuery query, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrWhiteSpace(userIdClaim))
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        var result = await query.ExecuteAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("transaction")]
    public async Task<IActionResult> GetTransaction([FromServices] GetCreditTransactionsQuery query,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrWhiteSpace(userIdClaim))
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        var result = await query.ExecuteAsync(userId, take, cancellationToken);
        return Ok(result);
    }
}
