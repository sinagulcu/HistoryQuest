

using HistoryQuest.Domain.Entities;

namespace HistoryQuest.Infrastructure.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task DeleteAsync(Category category, CancellationToken cancellationToken = default);
}
