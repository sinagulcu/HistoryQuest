

namespace HistoryQuest.Domain.Exceptions;

public class ForbiddenException(string message) : DomainException(message)
{
    public override int StatusCode => 403;
}
