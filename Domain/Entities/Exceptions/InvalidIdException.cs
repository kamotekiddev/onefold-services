namespace Domain.Entities.Exceptions;

public class InvalidIdException(string? message = null) : Exception(message);