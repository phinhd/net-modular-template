using Users.Application.Interfaces;

namespace Users.Application.Services;

/// <summary>
/// Placeholder used until real email/queue infra is wired up.
/// Swap this registration in DependencyInjection.cs for a real sender later
/// (still inside Application, e.g. an email queue producer) without touching Domain.
/// </summary>
public class NoOpEmailSender : IEmailSender
{
    public Task SendWelcomeEmailAsync(string toEmail, string fullName, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
