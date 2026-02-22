namespace HistoryQuest.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message)
{
    public abstract int StatusCode { get; }
}
