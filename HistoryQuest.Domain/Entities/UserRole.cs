namespace HistoryQuest.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    public User User { get; private set; }
    public Role Role { get; private set; }

    protected UserRole() { }

    public UserRole(User user, Role role)
    {
        User = user;
        Role = role;
        UserId = user.Id;
        RoleId = role.Id;
    }
}