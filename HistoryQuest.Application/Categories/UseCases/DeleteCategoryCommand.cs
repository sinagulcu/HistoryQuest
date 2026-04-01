

using HistoryQuest.Application.Categories.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Categories.UseCases;

public class DeleteCategoryCommand
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommand(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task ExecuteAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
            throw new NotFoundException("Category not found.");

        category.SoftDelete();
        await _categoryRepository.DeleteAsync(category, cancellationToken);
    }
}
