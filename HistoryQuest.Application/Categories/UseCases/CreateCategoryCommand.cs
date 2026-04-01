

using HistoryQuest.Application.Categories.DTOs;
using HistoryQuest.Application.Categories.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Categories.UseCases;

public class CreateCategoryCommand
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommand(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Guid> ExecuteAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await _categoryRepository.ExistsByNameAsync(request.Name, null, cancellationToken);
        if (exists)
            throw new BusinessRuleException("This category name use already.");

        var category = Category.Create(request.Name, request.Description);
        await _categoryRepository.AddAsync(category, cancellationToken);

        return category.Id;
    }
}
