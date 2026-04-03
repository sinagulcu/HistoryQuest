

using HistoryQuest.Application.Questions.DTOs;
using HistoryQuest.Application.Questions.Interfaces;

namespace HistoryQuest.Application.Questions.UseCases.Commands;

public class GetQuestionDeleteUsageQuery
{
    private readonly IQuestionRepository _questionRepository;

    public GetQuestionDeleteUsageQuery(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public Task<QuestionDeleteUsageDto> ExecuteAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        return _questionRepository.GetDeleteUsageAsync(questionId, cancellationToken);
    }
}
