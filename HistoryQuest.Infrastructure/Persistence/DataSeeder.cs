
using Azure.Identity;
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Enums;
using HistoryQuest.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace HistoryQuest.Infrastructure.Persistence;

public class DataSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var rolesToEnsure = new List<UserRoleType> { UserRoleType.Admin, UserRoleType.Teacher, UserRoleType.Student };

        foreach (var roleType in rolesToEnsure)
        {
            var roleExists = await roleRepository.ExistsAsync(roleType);
            if (!roleExists)
            {
                var role = new Role(roleType);
                await roleRepository.AddAsync(role);
            }
        }

        var adminExists = await userRepository.AnyUserInRoleAsync(UserRoleType.Admin);
        if (!adminExists)
        {
            var adminRole = await roleRepository.GetByTypeAsync(UserRoleType.Admin);

            var admin = new User(
                userName: "admin",
                firstName: "Admin",
                lastName: "HistoryQuest",
                email: "admin@historyquest.com",
                passwordHash: passwordHasher.Hash("St974097.!")
            );

            admin.AssignRole(adminRole);

            await userRepository.AddAsync(admin);
        }
    }
}
