using HistoryQuest.Domain.Models;

namespace HistoryQuest.Domain.Entities;

public class Role : BaseEntity
{
    public UserRoleType RoleType { get; private set; }

    protected Role() { }

    public Role(UserRoleType roleType)
    {
        RoleType = roleType;
    }
}
