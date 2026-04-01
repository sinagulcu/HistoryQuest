

using HistoryQuest.Application.Questions.DTOs;
using HistoryQuest.Application.Questions.Interfaces;

namespace HistoryQuest.Application.Questions.UseCases.Commands;

public class GetAllQuestionsCommand
{
    private readonly IQuestionRepository _questionRepository;

    public GetAllQuestionsCommand(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<List<QuestionListItemDto>> ExecuteAsync(bool includeDeleted = false)
    {
        return await _questionRepository.GetAllForAdminPanelAsync(includeDeleted);
    }
}
