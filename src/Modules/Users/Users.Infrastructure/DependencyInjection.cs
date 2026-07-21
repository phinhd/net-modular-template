using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("UsersDb")
                ?? configuration.GetConnectionString("DefaultConnection")));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailSender, NoOpEmailSender>();
        services.AddScoped<AuthService>();

        return services;
    }
}
