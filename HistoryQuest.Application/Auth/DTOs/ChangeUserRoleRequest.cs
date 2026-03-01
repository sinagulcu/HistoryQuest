

using HistoryQuest.Domain.Models;

namespace HistoryQuest.Application.Auth.DTOs;

public class ChangeUserRoleRequest
{
    public Guid UserId { get; set; }
    public UserRoleType NewRole { get; set; }
}
