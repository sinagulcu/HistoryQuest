

using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Application.Categories.Interfaces;
using HistoryQuest.Application.Challenges.Interfaces;
using HistoryQuest.Application.Credits.Interfaces;
using HistoryQuest.Application.Credits.Services;
using HistoryQuest.Application.Credits.UseCases;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Application.Users.Interfaces;
using HistoryQuest.Infrastructure.Persistence;
using HistoryQuest.Infrastructure.Repositories;
using HistoryQuest.Infrastructure.Security;
using HistoryQuest.Infrastructure.Services;
using HistoryQuest.Infrastructure.Services.CleanUp;
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
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IChallengeRepository, ChallengeRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenCleanupService, RefreshTokenCleanupService>();
        services.Configure<HardDeleteOptions>(configuration.GetSection("HardDelete"));

        services.AddScoped<IHardDeletePolicy, QuestionHardDeletePolicy>();
        services.AddScoped<IHardDeletePolicy, QuizHardDeletePolicy>();
        services.AddScoped<IHardDeletePolicy, CategoryHardDeletePolicy>();
        services.AddScoped<IHardDeletePolicy, TimedChallengeHardDeletePolicy>();

        services.AddHostedService<HardDeleteCleanupService>();
        services.AddHostedService<RefreshTokenCleanupHostedService>();

        services.AddScoped<IUserReadRepository, UserReadRepository>();

        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<ICreditTransactionRepository, CreditTransactionRepository>();


        return services;
    }
}
