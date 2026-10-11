namespace Users.Application.Interfaces;

/// <summary>
/// Non-business interface (no domain logic, just an outbound side-effect).
/// Per convention it lives in Application/Interfaces and its default implementation
/// (<see cref="Services.NoOpEmailSender"/>) lives in Application too. Swap the registration
/// for an SMTP or queue-backed sender when the project needs one.
/// </summary>
public interface IEmailSender
{
    Task SendWelcomeEmailAsync(string toEmail, string fullName, CancellationToken cancellationToken = default);
}
