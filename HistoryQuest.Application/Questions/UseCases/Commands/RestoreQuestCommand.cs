using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases.Commands;

public class RestoreQuestCommand
{
    private readonly IQuestionRepository _questionRepository;

    public RestoreQuestCommand(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task ExecuteAsync(Guid questionId, Guid userId, bool isAdmin)
    {
        var question = await _questionRepository.GetByIdIncludingDeletedAsync(questionId);
        if (question == null)
            throw new NotFoundException("Question not found or not deleted.");
        
        question.Restore(userId,isAdmin);
        await _questionRepository.SaveChangesAsync();
    }
}
