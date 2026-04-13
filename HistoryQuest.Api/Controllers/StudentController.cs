using HistoryQuest.Application.Questions.DTOs.Quiz;
using HistoryQuest.Application.Questions.UseCases.Quiz;
using HistoryQuest.Application.Questions.UseCases.Quiz.Commands;
using HistoryQuest.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Claims;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student")]
public class StudentController : ControllerBase
{
    private readonly StartQuizQueryHandler _startQuizHandler;
    private readonly SubmitQuizCommandHandler _submitQuizHandler;
    private readonly GetAttemptResultQueryHandler _getResultHandler;

    public StudentController(
        StartQuizQueryHandler startQuizHandler,
        SubmitQuizCommandHandler submitQuizController,
        GetAttemptResultQueryHandler getResultHandler)
    {
        _startQuizHandler = startQuizHandler;
        _submitQuizHandler = submitQuizController;
        _getResultHandler = getResultHandler;
    }

    [HttpGet("{quizId}/start")]
    public async Task<IActionResult> StartQuiz(Guid quizId)
    {
        Guid studentId;
        try
        {
            studentId = GetCurrentUserIdOrThrow();
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
        try
        {
            var result = await _startQuizHandler.Handle(new StartQuizQuery(quizId, studentId));
            return Ok(result);
        }
        catch (NotFoundException ex) { return NotFound(ex.Message); }
        catch (BusinessRuleException ex) {  return BadRequest(ex.Message); }
    }

    [HttpPost("{quizId}/submit")]
    public async Task<IActionResult> SubmitQuiz(Guid quizId, SubmitQuizRequest request)
    {
        Guid studentId;
        try
        {
            studentId = GetCurrentUserIdOrThrow();
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }


        var command = new SubmitQuizCommand(quizId, studentId,
            request.Answers.Select(a => new SubmitAnswerDto
            {
                QuestionId = a.QuestionId,
                SelectedOptionId = a.SelectedOptionId
            }).ToList());

        try
        {
            var result = await _submitQuizHandler.Handle(command);
            return Ok(result);
        }
        catch (NotFoundException ex) { return NotFound(ex.Message); }
        catch (BusinessRuleException ex) { return BadRequest(ex.Message); }
    }

    [HttpGet("result/{attemptId}")]
    public async Task<IActionResult> GetResult(Guid attemptId)
    {
        Guid studentId;
        try
        {
            studentId = GetCurrentUserIdOrThrow();
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }

        try
        {
            var result = await _getResultHandler.Handle(new GetAttemptResultQuery(attemptId, studentId));
            return Ok(result);
        }
        catch (NotFoundException ex) { return NotFound(ex.Message); }
        catch (UnauthorizedException ex) { return Forbid(); }
    }

    private Guid GetCurrentUserIdOrThrow()
    {
        var candidates = new[]
           {
        User.FindFirstValue(ClaimTypes.NameIdentifier),
        User.FindFirstValue("sub"),
        User.FindFirstValue("nameid")
    };

        foreach (var raw in candidates)
        {
            if (!string.IsNullOrWhiteSpace(raw) && Guid.TryParse(raw, out var id) && id != Guid.Empty)
                return id;
        }

        throw new UnauthorizedAccessException("Valid user id claim not found.");
    }
}
