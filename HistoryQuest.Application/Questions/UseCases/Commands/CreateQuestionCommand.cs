

using HistoryQuest.Application.Questions.DTOs;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Models;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases.Commands;

public class CreateQuestionCommand
{
    private readonly IQuestionRepository _questionRepository;

    public CreateQuestionCommand(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }
    public async Task<Guid> ExecuteAsync(
        CreateQuestionRequest request,
        Guid teacherId)
    {
        if (request.Options.Count < 2)
            throw new BusinessRuleException("Question must have at least 2 option.");

        if (request.Options.Count(o => o.IsCorrect) != 1)
            throw new BusinessRuleException("Question must have exactly one correct option.");

        var question = Question.Create(
            request.Text,
            request.Difficulty,
            request.Type,
            teacherId,
            request.CategoryId,   
            request.Explanation
         );

        foreach (var option in request.Options)
            question.AddOption(option.Text, option.IsCorrect);

        await _questionRepository.AddAsync(question);

        return question.Id;

    }
}
