
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
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QuizQuestion>()
            .HasKey(qq => new { qq.QuizId, qq.QuestionId });

        modelBuilder.Entity<QuizQuestion>()
            .HasOne(qq => qq.Quiz)
            .WithMany(q => q.QuizQuestions)
            .HasForeignKey(qq => qq.QuizId);

        modelBuilder.Entity<QuizQuestion>()
            .HasOne(qq => qq.Question)
            .WithMany()
            .HasForeignKey(qq => qq.QuestionId);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HistoryQuestDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
