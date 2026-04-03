using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases.Commands;

public class DeleteQuestionCommand
{
    private readonly IQuestionRepository _questionRepository;

    public DeleteQuestionCommand(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task ExecuteAsync(Guid questionId, Guid userId, bool isAdmin)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question == null)
            throw new NotFoundException("Question not found.");

        if (!isAdmin && question.CreatedByTeacherId != userId)
            throw new UnauthorizedException("You do not have permission to delete this question.");

        var usage = await _questionRepository.GetDeleteUsageAsync(questionId);
        if (!usage.CanDelete)
            throw new BusinessRuleException(usage.Message);

        question.DeleteByUser(userId, isAdmin);
        await _questionRepository.SaveChangesAsync();
    }
}
