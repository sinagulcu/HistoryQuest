
namespace HistoryQuest.Domain.Exceptions;

public class ConflictException(string message) : DomainException(message)
{
    public override int StatusCode => 409;
}
