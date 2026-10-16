namespace Domain.Entities.Exceptions;

public class CredentialAlreadyExistException(string message = "Credential already exists.") : DomainException(message);
