

using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Questions.UseCases;

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

        question.DeleteByUser(userId, isAdmin);

        await _questionRepository.SaveChangesAsync();
    }
}
