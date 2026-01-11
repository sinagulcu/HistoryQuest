namespace HistoryQuest.Domain.Entities;

public class User : BaseEntity
{
    public string UserName { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }

    private readonly List<UserRole> _roles  = new();
    public IReadOnlyCollection<UserRole> Roles  => _roles;

    protected User() {}

    public User(string userName, string firstName, string lastName, string email, string passwordHash)
    {
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public void AssignRole(Role role)
    {
        if (_roles.Any(r => r.RoleId == role.Id))
            return;

        _roles.Add(new UserRole(this, role));
    }
}