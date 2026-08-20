namespace BlogManagement.Exceptions;

public class InternalServerException : AppException
{
    public InternalServerException(string message) : base(message, 500)
    {
    }
}
