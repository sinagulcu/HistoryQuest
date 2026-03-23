

using HistoryQuest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HistoryQuest.Infrastructure.Persistence.Configurations;

public class AttemptAnswersConfiguration : IEntityTypeConfiguration<AttemptAnswer>
{
    public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsCorrect).IsRequired();
        builder.Property(x => x.SelectedOptionId).IsRequired();
        builder.Property(x => x.QuestionId).IsRequired();
    }
}
