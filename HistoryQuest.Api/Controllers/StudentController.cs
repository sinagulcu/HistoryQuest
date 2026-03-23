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
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var result = await _getResultHandler.Handle(new GetAttemptResultQuery(attemptId, studentId));
            return Ok(result);
        }
        catch (NotFoundException ex) { return NotFound(ex.Message); }
        catch (UnauthorizedException ex) { return Forbid(); }
    }
}
