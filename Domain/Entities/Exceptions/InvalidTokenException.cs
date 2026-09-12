namespace Domain.Entities.Exceptions;

public class InvalidTokenException(string? message = null) : Exception(message);