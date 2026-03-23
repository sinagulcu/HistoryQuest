

using HistoryQuest.Application.Questions.DTOs.Quiz;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace HistoryQuest.Application.Questions.UseCases.Quiz;

public class GetAttemptResultQueryHandler
{
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly IQuizRepository _quizRepository;

    public GetAttemptResultQueryHandler(IQuizAttemptRepository attemptRepository, IQuizRepository quizRepository)
    {
        _attemptRepository = attemptRepository;
        _quizRepository = quizRepository;
    }

    public async Task<AttemptResultResponse> Handle(GetAttemptResultQuery query)
    {
        var attempt = await _attemptRepository.GetByIdAsync(query.AttemptId);

        if (attempt is null)
            throw new NotFoundException("Answer not found.");

        if (attempt.StudentId != query.StudentId)
            throw new UnauthorizedException("Cannot permission this answer");

        var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId);

        return new AttemptResultResponse
        {
            Score = attempt.Score,
            TotalQuestions = attempt.TotalQuestions,
            Questions = quiz.QuizQuestions
            .OrderBy(qq => qq.Order)
            .Select(qq =>
            {
                var answer = attempt.Answers
                .FirstOrDefault(a => a.QuestionId == qq.Question.Id);

                return new ResultQuestionDto
                {
                    QuestionId = qq.Question.Id,
                    Text = qq.Question.Text,
                    Explanation = qq.Question.Explanation,
                    Options = qq.Question.Options.Select(o => new ResultOptionDto
                    {
                        OptionId = o.Id,
                        Text = o.Text,
                        IsCorrect = o.IsCorrect,
                        WasSelected = answer?.SelectedOptionId == o.Id
                    }).ToList()
                };
            }).ToList()
        };
    }
}
