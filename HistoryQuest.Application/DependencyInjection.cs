

using HistoryQuest.Application.Auth.UseCases;
using HistoryQuest.Application.Categories.UseCases;
using HistoryQuest.Application.Challenges.UseCases;
using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Application.Credits.Services;
using HistoryQuest.Application.Credits.UseCases;
using HistoryQuest.Application.Questions.UseCases.Commands;
using HistoryQuest.Application.Questions.UseCases.Quiz;
using HistoryQuest.Application.Questions.UseCases.Quiz.Commands;
using HistoryQuest.Application.Users.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace HistoryQuest.Application;

public static class DependencyInjection
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserCommand>();
        services.AddScoped<LoginUserQuery>();
        services.AddScoped<LogoutCommand>();
        services.AddScoped<RefreshTokenCommand>();
        services.AddScoped<ChangeUserRoleCommand>();
        services.AddScoped<DeleteUserCommand>();

        services.AddScoped<CreateCategoryCommand>();
        services.AddScoped<GetCategoriesQuery>();
        services.AddScoped<GetCategoryByIdQuery>();
        services.AddScoped<UpdateCategoryCommand>();
        services.AddScoped<DeleteCategoryCommand>();

        services.AddScoped<CreateChallengeCommand>();
        services.AddScoped<GetChallengesQuery>();
        services.AddScoped<GetChallengeByIdQuery>();
        services.AddScoped<UpdateChallengeCommand>();
        services.AddScoped<DeleteChallengeCommand>();

        services.AddScoped<CreateQuestionCommand>();
        services.AddScoped<DeleteQuestionCommand>();
        services.AddScoped<GetQuestionByIdCommand>();
        services.AddScoped<GetAllQuestionsCommand>();
        services.AddScoped<GetMyQuestionsCommand>();
        services.AddScoped<UpdateQuestionCommand>();
        services.AddScoped<RestoreQuestCommand>();
        services.AddScoped<GetQuestionDeleteUsageQuery>();

        services.AddScoped<CreateQuizCommand>();
        services.AddScoped<UpdateQuizCommand>();
        services.AddScoped<GetMyQuizzesCommand>();
        services.AddScoped<GetQuizDetailQuery>();
        services.AddScoped<AddQuestionToQuizCommand>();
        services.AddScoped<PublishQuizCommand>();
        services.AddScoped<UnpublishCommand>();
        services.AddScoped<SoftDeleteQuizCommand>();
        services.AddScoped<RestoreQuizCommand>();
        services.AddScoped<RemoveQuestionFromQuizCommand>();

        services.AddScoped<StartQuizQueryHandler>();
        services.AddScoped<SubmitQuizCommandHandler>();
        services.AddScoped<GetAttemptResultQueryHandler>();

        services.AddScoped<GetUsersQuery>();
        services.AddScoped<GetUserCountQuery>();


        services.AddScoped<ICreditLedgerService, CreditLedgerService>();
        services.AddScoped<GetWalletSummaryQuery>();
        services.AddScoped<GetCreditTransactionsQuery>();

        return services;
    }
}
