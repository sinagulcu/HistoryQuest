

using FluentValidation;
using HistoryQuest.Application.Questions.DTOs.Quiz;

namespace HistoryQuest.Application.Questions.Validators.Quiz;

public class UpdateQuizRequestValidator : AbstractValidator<UpdateQuizRequest>
{
    public UpdateQuizRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Quiz title cannot be empty.")
            .MinimumLength(3).WithMessage("Quiz title must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Quiz title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Quiz description cannot be empty.")
            .MinimumLength(10).WithMessage("Quiz description must be at least 10 characters long.")
            .MaximumLength(1000).WithMessage("Quiz description cannot exceed 1000 characters.");
    }
}
