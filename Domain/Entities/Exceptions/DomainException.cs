namespace Domain.Entities.Exceptions;

public class DomainException(string message) : Exception(message)
{
}