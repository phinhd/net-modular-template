using BuildingBlocks.Application.Interfaces;

namespace BuildingBlocks.Application.Common;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
