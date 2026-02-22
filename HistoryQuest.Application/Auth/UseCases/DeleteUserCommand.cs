
using HistoryQuest.Application.Auth.Interfaces;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Application.Auth.UseCases;

public class DeleteUserCommand
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommand(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task ExecuteAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if(user == null)
            throw new NotFoundException("User not found.");

        user.MarkAsDeleted();

        await _userRepository.UpdateAsync(user);
    }
}
