

using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Application.Categories.Interfaces;
using HistoryQuest.Application.Challenges.Interfaces;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Application.Users.Interfaces;
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

        services.AddScoped<RefreshTokenCleanupService>();
        services.AddScoped<RefreshTokenCleanupHostedService>();
        services.AddHostedService<QuestionCleanupService>();

        services.AddScoped<IUserReadRepository, UserReadRepository>();

        return services;
    }
}
