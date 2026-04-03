

using HistoryQuest.Application.Users.Interfaces;

namespace HistoryQuest.Application.Users.UseCases;

public sealed class GetUserCountQuery
{
    private readonly IUserReadRepository _userReadRepository;
    public GetUserCountQuery(IUserReadRepository userReadRepository)
    {
        _userReadRepository = userReadRepository;
    }

    public async Task<int> ExecuteAsync()
    {
        return await _userReadRepository.GetCountAsync();
    }
}
