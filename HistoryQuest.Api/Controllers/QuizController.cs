using HistoryQuest.Application.Questions.DTOs;
using HistoryQuest.Application.Questions.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly CreateQuizCommand _createQuizCommand;
    private readonly GetMyQuizzesCommand _getMyQuizzesCommand;
    private readonly AddQuestionToQuizCommand _addQuestionToQuizCommand;

    public QuizController(
        CreateQuizCommand createQuizCommand, 
        GetMyQuizzesCommand getMyQuizzesCommand,
        AddQuestionToQuizCommand addQuestionToQuizCommand)
    {
        _createQuizCommand = createQuizCommand;
        _getMyQuizzesCommand = getMyQuizzesCommand;
        _addQuestionToQuizCommand = addQuestionToQuizCommand;
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create(CreateQuizRequest request)
    {
        var teacherIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (teacherIdClaim == null)
            return Unauthorized();

        var teacherId = Guid.Parse(teacherIdClaim);

        var quizId = await _createQuizCommand.ExecuteAsync(
            request.Title,
            request.Description,
            teacherId);

        return CreatedAtAction(nameof(GetMyQuizzes), new { id = quizId }, quizId);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyQuizzes([FromQuery] bool includeDeleted = false)
    {
        var teacherIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (teacherIdClaim == null)
            return Unauthorized();

        var teacherId = Guid.Parse(teacherIdClaim);

        var quizzes = await _getMyQuizzesCommand.ExecuteAsync(teacherId, includeDeleted);

        var result = quizzes.Select(q => new QuizForTeacherDto
        {
            Id = q.Id,
            Title = q.Title,
            Description = q.Description,
            Status = q.Status.ToString(),
            QuestionCount = q.QuestionCount,
            IsDeleted = q.IsDeleted,
            DeletedAt = q.DeletedAt
        }).ToList();

        return Ok(result);
    }

    [HttpPost("{quizId}/questions/{questionId}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> AddQuestionToQuiz(Guid quizId, Guid questionId)
    {
        var teacherIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(teacherIdClaim == null) return Unauthorized();

        var teacherId = Guid.Parse(teacherIdClaim);

        await _addQuestionToQuizCommand.ExecuteAsync(quizId, questionId, teacherId);

        return Ok();
    }
}
