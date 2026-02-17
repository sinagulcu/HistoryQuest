using HistoryQuest.Application.Questions.DTOs;
using HistoryQuest.Application.Questions.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HistoryQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly CreateQuestionCommand _command;

    private readonly GetQuestionByIdQuery _getQuestionByIdQuery;
    private readonly GetMyQuestionsQuery _getMyQuestionsQuery;
    private readonly UpdateQuestionCommand _updateQuestionCommand;

    public QuestionsController(CreateQuestionCommand command, GetQuestionByIdQuery getQuestionByIdQuery, GetMyQuestionsQuery getMyQuestionsQuery, UpdateQuestionCommand updateQuestionCommand)
    {
        _command = command;
        _getQuestionByIdQuery = getQuestionByIdQuery;
        _getMyQuestionsQuery = getMyQuestionsQuery;
        _updateQuestionCommand = updateQuestionCommand;
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Create(CreateQuestionRequest request)
    {
        var teacherId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var questionId = await _command.ExecuteAsync(request, teacherId);

        return Ok(new { questionId });
    }

    [HttpGet("my")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyQuestion()
    {
        var teacherClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(teacherClaim == null)
            return Unauthorized();

        var teacherId = Guid.Parse(teacherClaim);

        var questions = await _getMyQuestionsQuery.ExecuteAsync(teacherId);

        var result = questions.Select(q => new QuestionForTeacherDto
        {
            Id = q.Id,
            Text = q.Text,
            Type = q.Type.ToString(),
            Difficulty = q.Difficulty.ToString(),
            Options = q.Options.Select(o => new OptionForTeacherDto
            {
                Id = o.Id,
                Text = o.Text,
                IsCorrect = o.IsCorrect
            }).ToList()
        }).ToList();

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Update(Guid id, UpdateQuestionRequest request)
    {
        var teacherIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (teacherIdClaim == null)
            return Unauthorized();

        var teacherId = Guid.Parse(teacherIdClaim);

        try
        {
            var result = await _updateQuestionCommand.ExecuteAsync(id, teacherId, request);
            if (!result)
                return NotFound();

            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var question = await _getQuestionByIdQuery.ExecuteAsync(id);

        if(question == null)
            return NotFound();

        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if(role == "Teacher")
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
