namespace BuildingBlocks.Application.Exceptions;

/// <summary>
/// Base type for expected, client-facing failures. The API layer maps each subtype
/// to an HTTP status code and an RFC 7807 problem response; anything else is a 500.
/// </summary>
public abstract class AppException : Exception
{
    protected AppException(string message) : base(message)
    {
    }
}

/// <summary>Thrown when a requested entity does not exist. Mapped to 404.</summary>
public sealed class NotFoundException : AppException
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}

/// <summary>Thrown for business rule violations. Mapped to 400.</summary>
public sealed class BusinessRuleException : AppException
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}

/// <summary>Thrown when the request conflicts with existing state (e.g. a duplicate email). Mapped to 409.</summary>
public sealed class ConflictException : AppException
{
    public ConflictException(string message) : base(message)
    {
    }
}

/// <summary>Thrown for authentication failures (bad credentials, inactive account). Mapped to 401.</summary>
public sealed class AuthenticationFailedException : AppException
{
    public AuthenticationFailedException(string message = "Invalid credentials.") : base(message)
    {
    }
}

/// <summary>Thrown when an authenticated caller is not allowed to perform the action. Mapped to 403.</summary>
public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message = "You do not have permission to perform this action.") : base(message)
    {
    }
}
