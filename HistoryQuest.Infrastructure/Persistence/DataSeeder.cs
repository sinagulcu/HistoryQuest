
using Azure.Identity;
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace HistoryQuest.Infrastructure.Persistence;

public class DataSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var adminRole = await roleRepository.GetByTypeAsync(UserRoleType.Admin);

        if(adminRole is null)
        {
            adminRole = new Role(UserRoleType.Admin);
            await roleRepository.AddAsync(adminRole);
        }

        var adminExists = await userRepository.AnyUserInRoleAsync(UserRoleType.Admin);

        if (adminExists)
            return;



        var admin = new User(
            userName: "admin",
            firstName: "System",
            lastName: "Administrator",
            email: "admin@historyquest.com",
            passwordHash: passwordHasher.Hash("St974097.")
            );

        admin.AssignRole(adminRole);

        await userRepository.AddAsync(admin);
    }
}
