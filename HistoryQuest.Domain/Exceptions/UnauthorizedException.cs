
namespace HistoryQuest.Domain.Exceptions;

public class UnauthorizedException(string message) : DomainException(message)
{
    public override int StatusCode => 401;
}
