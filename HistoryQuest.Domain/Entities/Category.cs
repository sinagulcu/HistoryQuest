

using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Domain.Entities;

public sealed class Category
{
    public Guid Id { get; private set; }
    public string Name { get;private set; } = string.Empty;
    public string? Description { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Category() { }

    private Category(string name, string? description)
    {
        SetName(name);
        SetDescription(description);

        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public static Category Create(string name, string? description)
    {
        return new Category(name, description);
    }

    public void Update(string name, string? description)
    {
        EnsureNotDeleted();

        SetName(name);
        SetDescription(description);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        EnsureNotDeleted();

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("Category is not deleted.");
        IsDeleted = false;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("Category name cannot be empty.");
        var normalized = name.Trim();

        if(normalized.Length < 2)
            throw new BusinessRuleException("Category name must be at least 2 characters long.");

        Name = normalized;
    }

    private void SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
            throw new BusinessRuleException("Cannot modify a deleted category.");
    }
}
