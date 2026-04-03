
namespace HistoryQuest.Application.Users.DTOs;

public class UserListItemDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
