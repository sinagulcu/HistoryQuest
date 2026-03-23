

using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Application.Auth.UseCases;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Application.Questions.UseCases.Commands;
using HistoryQuest.Application.Questions.UseCases.Quiz;
using HistoryQuest.Application.Questions.UseCases.Quiz.Commands;
using HistoryQuest.Infrastructure.Persistence;
using HistoryQuest.Infrastructure.Repositories;
using HistoryQuest.Infrastructure.Security;
using HistoryQuest.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HistoryQuest.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<HistoryQuestDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRefreshTokenCleanupService, RefreshTokenCleanupService>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<GetQuizDetailQuery>();
        services.AddScoped<CreateQuestionCommand>();
        services.AddScoped<LogoutCommand>();
        services.AddScoped<RefreshTokenCommand>();
        services.AddScoped<RefreshTokenCleanupService>();
        services.AddScoped<RefreshTokenCleanupHostedService>();
        services.AddHostedService<QuestionCleanupService>();
        services.AddScoped<ChangeUserRoleCommand>();
        services.AddScoped<DeleteQuestionCommand>();
        services.AddScoped<DeleteUserCommand>();
        services.AddScoped<GetQuestionByIdCommand>();
        services.AddScoped<GetMyQuestionsCommand>();
        services.AddScoped<UpdateQuestionCommand>();
        services.AddScoped<RestoreQuestCommand>();
        services.AddScoped<CreateQuizCommand>();
        services.AddScoped<GetMyQuizzesCommand>();
        services.AddScoped<AddQuestionToQuizCommand>();
        services.AddScoped<PublishQuizCommand>();
        services.AddScoped<UnpublishCommand>();
        services.AddScoped<SoftDeleteQuizCommand>();
        services.AddScoped<RestoreQuizCommand>();
        services.AddScoped<RemoveQuestionFromQuizCommand>();

        services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
        services.AddScoped<StartQuizQueryHandler>();
        services.AddScoped<SubmitQuizCommandHandler>();
        services.AddScoped<GetAttemptResultQueryHandler>();


        return services;
    }
}
