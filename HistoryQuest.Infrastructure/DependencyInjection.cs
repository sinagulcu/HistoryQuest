

using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Application.Auth.UseCases;
using HistoryQuest.Application.Questions.Interfaces;
using HistoryQuest.Application.Questions.UseCases;
using HistoryQuest.Infrastructure.Persistence;
using HistoryQuest.Infrastructure.Repositories;
using HistoryQuest.Infrastructure.Security;
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
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<CreateQuestionCommand>();
        services.AddScoped<RegisterTeacherCommand>();
        services.AddScoped<DeleteUserCommand>();
        services.AddScoped<GetQuestionByIdQuery>();
        services.AddScoped<GetMyQuestionsQuery>();
        services.AddScoped<UpdateQuestionCommand>();

        return services;
    }
}
