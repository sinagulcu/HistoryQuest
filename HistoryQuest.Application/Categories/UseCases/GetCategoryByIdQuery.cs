
using HistoryQuest.Application.Categories.DTOs;
using HistoryQuest.Application.Categories.Interfaces;

namespace HistoryQuest.Application.Categories.UseCases;

public class GetCategoryByIdQuery
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQuery(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto?> ExecuteAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
            return null;
        
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt
        };
    }
}
