namespace Domain.Entities.Exceptions;

public class InvalidTokenExpirationException(string? message = null) : Exception(message);