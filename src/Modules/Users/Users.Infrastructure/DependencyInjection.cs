using BuildingBlocks.Application.Common;
using BuildingBlocks.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Users.Application.Interfaces;
using Users.Application.Services;
using Users.Domain.Interfaces;
using Users.Infrastructure.Auth;
using Users.Infrastructure.Persistence;

namespace Users.Infrastructure;

/// <summary>
/// Registers everything the Users module needs. The Host only calls
/// services.AddUsersModule(configuration) — it never sees this module's internals.
/// Copy this file's shape when you add a new module (Payroll, Recruitment, ...).
/// </summary>
public static class DependencyInjection
{
    public const string ConnectionStringName = "UsersDb";

    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' is not configured.");

        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", UsersDbContext.Schema)));

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Shared building blocks: TryAdd so several modules can register them safely.
        services.TryAddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailSender, NoOpEmailSender>();
        services.AddScoped<AuthService>();
        services.AddScoped<UserService>();

        return services;
    }
}
