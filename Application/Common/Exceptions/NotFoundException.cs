namespace Application.Common.Exceptions;

public class NotFoundException(string message) : KeyNotFoundException(message)
{
}