namespace NewsAgency.Domain.Exceptions;

public class InvalidValueObjectStateException : Exception
{
    public InvalidValueObjectStateException(string message) : base(message)
    {
    }
}
