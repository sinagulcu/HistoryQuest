

namespace HistoryQuest.Domain.Exceptions;

public class NotFoundException(string message) : DomainException(message)
{
    public override int StatusCode => 404;
}
