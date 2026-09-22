namespace Application.Common.Exceptions;

public class UnAuthorizedException(string message) : Exception(message)
{
}