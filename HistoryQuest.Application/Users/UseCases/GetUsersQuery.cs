
using HistoryQuest.Application.Users.DTOs;
using HistoryQuest.Application.Users.Interfaces;

namespace HistoryQuest.Application.Users.UseCases;

public sealed class GetUsersQuery
{
    private readonly IUserReadRepository _userReadRepository;

    public GetUsersQuery(IUserReadRepository userReadRepository)
    {
        _userReadRepository = userReadRepository;
    }

    public async Task<List<UserListItemDto>> ExecuteAsync()
    {
        return await _userReadRepository.GetAllAsync();
    }
}
