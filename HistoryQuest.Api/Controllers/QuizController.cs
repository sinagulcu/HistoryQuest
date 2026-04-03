using HistoryQuest.Application.Questions.DTOs;
using HistoryQuest.Application.Questions.DTOs.Quiz;
using HistoryQuest.Application.Questions.UseCases.Commands;
using HistoryQuest.Application.Questions.UseCases.Quiz;
using HistoryQuest.Application.Questions.UseCases.Quiz.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
    private readonly GetQuizDetailQuery _getQuizDetailQuery;
    private readonly PublishQuizCommand _publishQuizCommand;
    private readonly UnpublishCommand _unpublishQuizCommand;
    private readonly SoftDeleteQuizCommand _softDeleteQuizCommand;
    private readonly RestoreQuizCommand _restoreQuizCommand;
    private readonly RemoveQuestionFromQuizCommand _removeQuestionFromQuizCommand;
    private readonly UpdateQuizCommand _updateQuizCommand;

    public QuizController(
        CreateQuizCommand createQuizCommand,
        GetMyQuizzesCommand getMyQuizzesCommand,
        AddQuestionToQuizCommand addQuestionToQuizCommand,
        GetQuizDetailQuery getQuizDetailQuery,
        PublishQuizCommand publishQuizCommand,
        UnpublishCommand unpublishQuizCommand,
        SoftDeleteQuizCommand softDeleteQuizCommand,
        RestoreQuizCommand restoreQuizCommand,
        RemoveQuestionFromQuizCommand removeQuestionFromQuizCommand,
        UpdateQuizCommand updateQuizCommand)
    {
        _createQuizCommand = createQuizCommand;
        _getMyQuizzesCommand = getMyQuizzesCommand;
        _addQuestionToQuizCommand = addQuestionToQuizCommand;
        _getQuizDetailQuery = getQuizDetailQuery;
        _publishQuizCommand = publishQuizCommand;
        _unpublishQuizCommand = unpublishQuizCommand;
        _softDeleteQuizCommand = softDeleteQuizCommand;
        _restoreQuizCommand = restoreQuizCommand;
        _removeQuestionFromQuizCommand = removeQuestionFromQuizCommand;
        _updateQuizCommand = updateQuizCommand;
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create(CreateQuizRequest request)
    {
        var teacherIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (teacherIdClaim == null) return Unauthorized();

        var teacherId = Guid.Parse(teacherIdClaim);
        var quizId = await _createQuizCommand.ExecuteAsync(request, teacherId);

        return CreatedAtAction(nameof(GetQuizDetails), new { quizId }, new { quizId });
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateQuizRequest request)
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim))
            return Unauthorized();

        var currentUserId = Guid.Parse(userIdClaim);
        var isAdmin = User.IsInRole("Admin");

        await _updateQuizCommand.ExecuteAsync(id, currentUserId, isAdmin, request);
        return Ok();
    }

    [HttpGet("GetMyQuizzes")]
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
            CategoryId = q.CategoryId,
            CategoryName = q.Category != null ? q.Category.Name : null,
            CreatedTeacherId = q.CreatedByTeacherId,
            TimeLimitMinutes = q.TimeLimitMinutes,
            CreatedTeacherUserName = q.CreatedByTeacher != null ? q.CreatedByTeacher.UserName : null,
            CreatedTeacherFullName = q.CreatedByTeacher != null
             ? $"{q.CreatedByTeacher.FirstName} {q.CreatedByTeacher.LastName}".Trim() : null,
            Status = q.Status.ToString(),
            QuestionCount = q.QuestionCount,
            IsDeleted = q.IsDeleted,
            DeletedAt = q.DeletedAt
        }).ToList();

        return Ok(result);
    }

    [HttpPost("AddQuestionToQuiz/{quizId}/questions/{questionId}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> AddQuestionToQuiz(Guid quizId, Guid questionId)
    {
        var teacherIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (teacherIdClaim == null) return Unauthorized();

        var teacherId = Guid.Parse(teacherIdClaim);

        await _addQuestionToQuizCommand.ExecuteAsync(quizId, questionId, teacherId);

        return Ok();
    }

    [HttpGet("QuizDetail/{quizId}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetQuizDetails(Guid quizId)
    {
        var quiz = await _getQuizDetailQuery.ExecuteAsync(quizId);
        if (quiz == null) return NotFound();

        var result = new QuizDetailDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            CategoryId = quiz.CategoryId,
            CategoryName = quiz.Category != null ? quiz.Category.Name : null,
            CreatedTeacherId = quiz.CreatedByTeacherId,
            CreatedTeacherUserName = quiz.CreatedByTeacher != null ? quiz.CreatedByTeacher.UserName : null,
            CreatedTeacherFullName = quiz.CreatedByTeacher != null
                ? $"{quiz.CreatedByTeacher.FirstName} {quiz.CreatedByTeacher.LastName}".Trim() : null,
            QuizQuestions = [.. quiz.QuizQuestions
            .OrderBy(q => q.Order)
            .Select(q => new QuizQuestionDto
            {
                QuestionId = q.QuestionId,
                QuestionText = q.Question.Text,
                Order = q.Order,
                IsDeleted = q.Question.IsDeleted,
            })]
        };
        return Ok(result);
    }

    [HttpPost("{quizId}/publish")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> PublishQuiz(Guid quizId)
    {
        var teacherIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (teacherIdClaim == null) return Unauthorized();

        var teacherId = Guid.Parse(teacherIdClaim);
        await _publishQuizCommand.ExecuteAsync(quizId, teacherId);

        return Ok();
    }

    [HttpPost("{quizId}/revert")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UnpublishQuiz(Guid quizId)
    {
        var teacherIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (teacherIdClaim == null) return Unauthorized();

        var teacherId = Guid.Parse(teacherIdClaim);
        await _unpublishQuizCommand.ExecuteAsync(quizId, teacherId);

        return Ok();
    }

    [HttpPost("{quizId}/delete")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> SoftDeleteQuiz(Guid quizId)
    {
        var teacherId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _softDeleteQuizCommand.ExecuteAsync(quizId, teacherId);

        return Ok();
    }

    [HttpPost("{quizId}/restore")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> RestoreQuiz(Guid quizId)
    {
        var teacherId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _restoreQuizCommand.ExecuteAsync(quizId, teacherId);
        return Ok();
    }

    [HttpDelete("{quizId}/questions/{questionId}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> RemoveQuestionFromQuiz(Guid quizId, Guid questionId)
    {
        var teacherId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _removeQuestionFromQuizCommand.ExecuteAsync(quizId, questionId, teacherId);

        return Ok();
    }
}
