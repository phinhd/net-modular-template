namespace BuildingBlocks.Application.Interfaces;

/// <summary>
/// Non-business (infrastructure-flavored but not business-rule) interface.
/// Per convention: these live in Application/Interfaces and are implemented
/// directly inside Application (see Common/SystemDateTimeProvider.cs), NOT in Infrastructure.
/// Same spot for things like IEmailSender, IFileStorage if their implementation
/// doesn't need EF/DbContext.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
