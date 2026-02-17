

using HistoryQuest.Application.Questions.DTOs;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Enums;

namespace HistoryQuest.Application.Questions.UseCases;

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
            throw new Exception("Question must have at least 2 option.");

        if (request.Options.Count(o => o.IsCorrect) != 1)
            throw new Exception("Question must have exactly one correct option.");

        var question = Question.Create(
            request.Text,
            QuestionDifficulty.Medium,
            QuestionType.SingleChoice,
            teacherId
         );

        foreach (var option in request.Options)
            question.AddOption(option.Text, option.IsCorrect);

        await _questionRepository.AddAsync(question);

        return question.Id;

    }
}
