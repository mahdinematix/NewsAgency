namespace NewsAgency.Domain.Exceptions;

public class DomainStateException : Exception
{
    public DomainStateException(string message):base(message)
    {
    }
}
