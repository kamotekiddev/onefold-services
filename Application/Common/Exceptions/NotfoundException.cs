namespace Application.Common.Exceptions;

public class NotfoundException(string message) : KeyNotFoundException(message)
{
}