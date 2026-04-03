using HistoryQuest.Application.Questions.DTOs;
using HistoryQuest.Application.Questions.UseCases.Commands;
using HistoryQuest.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly CreateQuestionCommand _command;

    private readonly GetQuestionByIdCommand _getQuestionByIdQuery;
    private readonly GetMyQuestionsCommand _getMyQuestionsQuery;
    private readonly UpdateQuestionCommand _updateQuestionCommand;
    private readonly DeleteQuestionCommand _deleteQuestionCommand;
    private readonly RestoreQuestCommand _restoreQuestionCommand;
    private readonly GetAllQuestionsCommand _getAllQuestionsCommand;

    public QuestionsController(
        CreateQuestionCommand command, 
        GetQuestionByIdCommand getQuestionByIdQuery,
        GetMyQuestionsCommand getMyQuestionsQuery, 
        UpdateQuestionCommand updateQuestionCommand, 
        DeleteQuestionCommand deleteQuestionCommand,
        RestoreQuestCommand restoreQuestCommand,
        GetAllQuestionsCommand getAllQuestionsCommand)
    {
        _command = command;
        _getQuestionByIdQuery = getQuestionByIdQuery;
        _getMyQuestionsQuery = getMyQuestionsQuery;
        _updateQuestionCommand = updateQuestionCommand;
        _deleteQuestionCommand = deleteQuestionCommand;
        _restoreQuestionCommand = restoreQuestCommand;
        _getAllQuestionsCommand = getAllQuestionsCommand;
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create(CreateQuestionRequest request)
    {
        var teacherId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var questionId = await _command.ExecuteAsync(request, teacherId);

        return Ok(new { questionId });
    }

    [HttpGet]
    [Authorize(Roles ="Teacher,Admin")]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
    {
        var isAdmin = User.IsInRole("Admin");

        if(!isAdmin) includeDeleted = false;

        var result = await _getAllQuestionsCommand.ExecuteAsync(includeDeleted);

        return Ok(result);
    }

    [HttpGet("getmyquestions")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyQuestion([FromQuery] bool includeDeleted = false)
    {
        var teacherIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (teacherIdClaim == null) return Unauthorized();

        var teacherId = Guid.Parse(teacherIdClaim);
        var questions = await _getMyQuestionsQuery.ExecuteAsync(teacherId, includeDeleted);

        var result = questions.Select(q => new QuestionForTeacherDto
        {
            Id = q.Id,
            Text = q.Text,
            Type = q.Type.ToString(),
            Difficulty = q.Difficulty.ToString(),
            CreatedAt = q.CreatedAt,
            IsDeleted = q.IsDeleted,
            DeletedAt = q.DeletedAt,
            DaysUntilHardDelete = q.DeletedAt.HasValue
            ? 30 - (int)(DateTime.UtcNow - q.DeletedAt.Value).TotalDays
            : null,
            Options = [.. q.Options.Select(o => new OptionForTeacherDto
            {
                Id = o.Id,
                Text = o.Text,
                IsCorrect = o.IsCorrect
            })]
        }).ToList();

        return Ok(result);
    }

    [HttpPut("update/{id}")]
    [Authorize(Roles = "Teacher, Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateQuestionRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var currentUserId = Guid.Parse(userIdClaim);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            var result = await _updateQuestionCommand.ExecuteAsync(id, currentUserId, isAdmin, request);
            if (!result) return NotFound();
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _deleteQuestionCommand.ExecuteAsync(id, userId, isAdmin);
            return NoContent();
        }
        catch (UnauthorizedException)
        {
            return Forbid();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("restore/{id}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Restore(Guid id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _restoreQuestionCommand.ExecuteAsync(id, userId, isAdmin);
            return NoContent();
        }
        catch (UnauthorizedException)
        {
            return Forbid();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var question = await _getQuestionByIdQuery.ExecuteAsync(id);

        if (question == null)
            return NotFound();

        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role == "Teacher")
        {
            var teacherDto = new QuestionForTeacherDto
            {
                Id = question.Id,
                Text = question.Text,
                Type = question.Type.ToString(),
                Difficulty = question.Difficulty.ToString(),
                Options = question.Options.Select(o => new OptionForTeacherDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            };

            return Ok(teacherDto);
        }
        else
        {
            var studentDto = new QuestionForStudentDto
            {
                Id = question.Id,
                Text = question.Text,
                Type = question.Type.ToString(),
                Difficulty = question.Difficulty.ToString(),
                Options = question.Options.Select(o => new OptionForStudentDto
                {
                    Id = o.Id,
                    Text = o.Text
                }).ToList()
            };
            return Ok(studentDto);
        }
    }

}
