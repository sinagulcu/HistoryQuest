

using HistoryQuest.Application.Questions.DTOs.Quiz;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;
using HistoryQuest.Domain.Models;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace HistoryQuest.Application.Questions.UseCases.Quiz;

public class StartQuizQueryHandler
{
    private readonly IQuizRepository _quizRepository;

    public StartQuizQueryHandler(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task<StartQuizResponse> Handle(StartQuizQuery query)
    {
        var quiz = await _quizRepository.GetByIdAsync(query.QuizId);

        if (quiz is null)
            throw new NotFoundException("Quiz not found.");

        if (quiz.Status != QuizStatus.Published)
            throw new BusinessRuleException("Quiz not published.");

        return new StartQuizResponse
        {
            QuizId = quiz.Id,
            Title = quiz.Title,
            Questions = quiz.QuizQuestions
            .OrderBy(qq => qq.Order)
            .Select(qq => new QuestionDto
            {
                QuestionId = qq.Question.Id,
                Text = qq.Question.Text,
                Options = qq.Question.Options.Select(o => new OptionDto
                {
                    OptionId = o.Id,
                    Text = o.Text
                }).ToList()
            }).ToList()
        };
    }
}
