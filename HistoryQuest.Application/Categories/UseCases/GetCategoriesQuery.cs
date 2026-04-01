

using HistoryQuest.Application.Categories.DTOs;
using HistoryQuest.Application.Categories.Interfaces;

namespace HistoryQuest.Application.Categories.UseCases;

public class GetCategoriesQuery
{
    private readonly ICategoryRepository _categoryRepository;
    
        public GetCategoriesQuery(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
    
        public async Task<List<CategoryDto>> ExecuteAsync(bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllAsync(includeDeleted, cancellationToken);

        return [.. categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt
            })];

    }
}
