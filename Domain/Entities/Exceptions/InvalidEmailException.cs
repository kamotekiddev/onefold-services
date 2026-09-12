namespace Domain.Entities.Exceptions;

public class InvalidEmailException(string? message = null) : Exception(message);