using BuildingBlocks.Domain.Common;

namespace Users.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string Role { get; private set; } = "Employee";
    public bool IsActive { get; private set; } = true;

    private User()
    {
        // EF Core
    }

    public static User Create(string email, string passwordHash, string fullName, string role = "Employee")
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        return new User
        {
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            FullName = fullName,
            Role = role
        };
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }

    public void UpdateProfile(string fullName)
    {
        FullName = fullName;
        MarkUpdated();
    }
}
