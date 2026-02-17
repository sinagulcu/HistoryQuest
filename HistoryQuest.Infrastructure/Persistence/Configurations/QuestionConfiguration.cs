
using HistoryQuest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HistoryQuest.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasMany(q => q.Options)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
