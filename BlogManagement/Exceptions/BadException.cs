namespace BlogManagement.Exceptions;

public class BadException : AppException
{
    public BadException(string message) : base(message, 400)
    {
    }
}
