using HistoryQuest.Application.Challenges.DTOs;
using HistoryQuest.Application.Challenges.UseCases;
using HistoryQuest.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Teacher ,Admin")]
public class ChallengeController : ControllerBase
{
    private readonly CreateChallengeCommand _createChallengeCommand;
    private readonly GetChallengesQuery _getChallengesQuery;
    private readonly GetChallengeByIdQuery _getChallengeByIdQuery;
    private readonly UpdateChallengeCommand _updateChallengeCommand;
    private readonly DeleteChallengeCommand _deleteChallengeCommand;

    public ChallengeController(
        CreateChallengeCommand createChallengeCommand,
        GetChallengesQuery getChallengesQuery,
        GetChallengeByIdQuery getChallengeByIdQuery,
        UpdateChallengeCommand updateChallengeCommand,
        DeleteChallengeCommand deleteChallengeCommand)
    {
        _createChallengeCommand = createChallengeCommand;
        _getChallengesQuery = getChallengesQuery;
        _getChallengeByIdQuery = getChallengeByIdQuery;
        _updateChallengeCommand = updateChallengeCommand;
        _deleteChallengeCommand = deleteChallengeCommand;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        var result = await _getChallengesQuery.ExecuteAsync(currentUserId, isAdmin, includeDeleted, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        var result = await _getChallengeByIdQuery.ExecuteAsync(id, currentUserId, isAdmin, cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create([FromBody] CreateChallengeRequest request, CancellationToken cancellationToken = default)
    {
        var teacherId = GetCurrentUserId();
        var challengeId = await _createChallengeCommand.ExecuteAsync(request, teacherId, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = challengeId }, new { id = challengeId });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChallengeRequest request, CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        await _updateChallengeCommand.ExecuteAsync(id, request, currentUserId, isAdmin, cancellationToken);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");

        await _deleteChallengeCommand.ExecuteAsync(id, currentUserId, isAdmin, cancellationToken);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim))
            throw new UnauthorizedException("User ID not found.");

        return Guid.Parse(userIdClaim);
    }
}
