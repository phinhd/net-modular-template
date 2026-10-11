using BuildingBlocks.Domain.Common;

namespace Users.Domain.Entities;

public sealed class User : BaseEntity, IAggregateRoot
{
    public const string DefaultRole = "Employee";

    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string Role { get; private set; } = DefaultRole;
    public bool IsActive { get; private set; } = true;

    private User()
    {
        // EF Core
    }

    public static User Create(string email, string passwordHash, string fullName, string role = DefaultRole)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        return new User
        {
            Email = NormalizeEmail(email),
            PasswordHash = passwordHash,
            FullName = fullName.Trim(),
            Role = role
        };
    }

    /// <summary>Single source of truth for how emails are compared and stored.</summary>
    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash is required.", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        MarkUpdated();
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        MarkUpdated();
    }

    public void UpdateProfile(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        FullName = fullName.Trim();
        MarkUpdated();
    }
}
