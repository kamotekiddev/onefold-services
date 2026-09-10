namespace Domain.Entities.Exceptions;

public class CredentialAlreadyExistException(string? message = null) : Exception(message);