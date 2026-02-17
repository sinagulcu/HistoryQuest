
using HistoryQuest.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Persistence;

public class HistoryQuestDbContext : DbContext
{
    public HistoryQuestDbContext(DbContextOptions<HistoryQuestDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Question> Questions => Set<Question>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HistoryQuestDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
