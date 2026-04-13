
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

    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<CreditTransaction> CreditTransaction => Set<CreditTransaction>();

    public DbSet<QuizEconomyRule> QuizEconomyRules => Set<QuizEconomyRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Question>(builder =>
        {
            builder.HasOne(q => q.CreatedByTeacher)
                .WithMany()
                .HasForeignKey(q => q.CreatedByTeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(q => q.Category)
                .WithMany()
                .HasForeignKey(q => q.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Quiz>(builder =>
        {
            builder.HasOne(q => q.CreatedByTeacher)
                .WithMany()
                .HasForeignKey(q => q.CreatedByTeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(q => q.Category)
                .WithMany()
                .HasForeignKey(q => q.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<QuizQuestion>()
            .HasKey(qq => new { qq.QuizId, qq.QuestionId });

        modelBuilder.Entity<QuizQuestion>()
            .HasOne(qq => qq.Quiz)
            .WithMany(q => q.QuizQuestions)
            .HasForeignKey(qq => qq.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuizQuestion>()
            .HasOne(qq => qq.Question)
            .WithMany()
            .HasForeignKey(qq => qq.QuestionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Wallet>(builder =>
        {
            builder.HasIndex(x => x.UserId).IsUnique();
            builder.Property(x => x.Balance).IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<CreditTransaction>(builder =>
        {
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CreatedAt);

            builder.HasIndex(x => x.IdempotencyKey)
                .IsUnique()
                .HasFilter("[IdempotencyKey] IS NOT NULL");
        });

        modelBuilder.Entity<QuizEconomyRule>(builder =>
        {
            builder.ToTable("QuizEconomyRules");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.QuizId).IsUnique();
            builder.Property(x => x.EntryCost).IsRequired();
            builder.Property(x => x.RewardPool).IsRequired();
            builder.Property(x => x.WrongPenaltyPerQuestion).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();
        });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HistoryQuestDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
