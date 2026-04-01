
using HistoryQuest.Application.Categories.DTOs;
using HistoryQuest.Application.Categories.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Categories.UseCases;

public class UpdateCategoryCommand
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommand(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task ExecuteAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
            throw new NotFoundException("Category not found.");

        var exists = await _categoryRepository.ExistsByNameAsync(request.Name, categoryId, cancellationToken);

        if (exists)
            throw new BusinessRuleException("This category use already.");

        category.Update(request.Name, request.Description);

        await _categoryRepository.UpdateAsync(category, cancellationToken);
    }
}
