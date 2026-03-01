

using HistoryQuest.Application.Auth.DTOs;
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Entities;
using HistoryQuest.Domain.Models;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Auth.UseCases;

public class ChangeUserRoleCommand
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    public ChangeUserRoleCommand(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;   
    }

    public async Task ExecuteAsync(ChangeUserRoleRequest request,
        User currentUser)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if(user is null)
            throw new NotFoundException($"User with id {request.UserId} not found.");

        

        var newRole = await _roleRepository.GetByTypeAsync(request.NewRole);
        if (newRole is null)
            throw new NotFoundException($"Role {request.NewRole} not found.");

        var currentUserRoles = currentUser.Roles
            .Where(r => r.Role != null)
            .Select(r => r.Role.RoleType)
            .ToList();

        if(currentUserRoles.Contains(UserRoleType.Admin))
            user.AssignRole(newRole);
        else if(currentUserRoles.Contains(UserRoleType.Teacher))
        {
            if(request.NewRole != UserRoleType.Student)
                throw new UnauthorizedException("Teachers can only assign Student role.");

            user.AssignRole(newRole);
        }
        else
            throw new UnauthorizedException("Only Admins and Teachers can change user roles.");

        await _userRepository.UpdateAsync(user);
    }
}
