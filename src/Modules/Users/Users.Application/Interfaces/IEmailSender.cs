namespace Users.Application.Interfaces;

/// <summary>
/// Non-business interface (no domain logic, just an outbound side-effect).
/// Per convention: lives in Application/Interfaces and is implemented directly
/// inside Application (see Services/SmtpEmailSender.cs or a queue-based stub),
/// NOT in Infrastructure.
/// </summary>
public interface IEmailSender
{
    Task SendWelcomeEmailAsync(string toEmail, string fullName, CancellationToken cancellationToken = default);
}
