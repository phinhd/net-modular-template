using BuildingBlocks.Application.Interfaces;

namespace BuildingBlocks.Application.Common;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
