

using HistoryQuest.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuest.Infrastructure.Persistence.Configurations;

public class TimedChallengeConfiguration : IEntityTypeConfiguration<TimedChallenge>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TimedChallenge> builder)
    {
        builder.ToTable("TimedChallenges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.QuestionId)
            .IsRequired();

        builder.Property(x => x.CreatedByTeacherId)
            .IsRequired();

        builder.Property(x => x.ScheduledAtUtc)
            .IsRequired();

        builder.Property(x => x.AnswerWindowSeconds)
            .IsRequired();

        builder.Property(x => x.VisibilityWindowSeconds)
            .IsRequired();

        builder.Property(x => x.MaxScore)
            .IsRequired();

        builder.Property(x => x.ShowCorrectAnswerOnWrong)
            .IsRequired();

        builder.Property(x => x.ShowExplanationOnWrong)
            .IsRequired();

        builder.Property(x => x.NotifyAllStudents)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => x.ScheduledAtUtc);
        builder.HasIndex(x => x.CreatedByTeacherId);
        builder.HasIndex(x => x.QuestionId);

        builder.HasOne<Question>()
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedByTeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
