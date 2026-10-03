namespace NewsAgency.Domain.Exceptions;
public class InvalidEntityStateException : Exception
{
    public InvalidEntityStateException(string message) : base(message)
    {
    }
}
