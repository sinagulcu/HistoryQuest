
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

    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<AttemptAnswer> AttemptAnswers => Set<AttemptAnswer>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<TimedChallenge> TimedChallenges => Set<TimedChallenge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.CreatedByTeacher)
            .WithMany()
            .HasForeignKey(q => q.CreatedByTeacherId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.Category)
            .WithMany()
            .HasForeignKey(q => q.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Category)
            .WithMany()
            .HasForeignKey(q => q.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

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
