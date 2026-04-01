
using FluentValidation;
using HistoryQuest.Application.Challenges.DTOs;

namespace HistoryQuest.Application.Challenges.Validators;

public class CreateChallengeRequestValidator : AbstractValidator<CreateChallengeRequest>
{
    public CreateChallengeRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(150).WithMessage("Title must be at most 150 characters.");
        
        RuleFor(x => x.QuestionId)
            .NotEqual(Guid.Empty).WithMessage("Question selection is required.");
        
        RuleFor(x => x.ScheduledAtUtc)
            .Must(x => x > DateTime.UtcNow).WithMessage("ScheduledAtUtc must be in the future.");

        RuleFor(x => x.AnswerWindowSeconds)
            .GreaterThanOrEqualTo(30).WithMessage("Points question must be greater than 30.");
        
        RuleFor(x => x.VisibilityWindowSeconds)
            .GreaterThanOrEqualTo(x => x.AnswerWindowSeconds).WithMessage("Broadcast duration cannot be shorter than the rated broadcast time.");

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage("Max score must be greater than 0.");
    }
}
