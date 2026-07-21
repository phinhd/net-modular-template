namespace BuildingBlocks.Application.Exceptions;

/// <summary>Thrown when a requested entity does not exist. Map to 404 in the API layer.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}

/// <summary>Thrown for business rule violations. Map to 400 in the API layer.</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}

/// <summary>Thrown for authentication failures (bad credentials, expired token, etc). Map to 401.</summary>
public class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException(string message = "Invalid credentials.") : base(message)
    {
    }
}
