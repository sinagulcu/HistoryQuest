using HistoryQuest.Domain.Enums;

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
