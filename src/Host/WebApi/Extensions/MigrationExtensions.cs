using Microsoft.EntityFrameworkCore;
using Users.Infrastructure.Persistence;

namespace WebApi.Extensions;

public static class MigrationExtensions
{
    /// <summary>
    /// Applies pending migrations for every module DbContext. Enabled via
    /// "Database:ApplyMigrationsOnStartup" — handy for local dev and Docker Compose;
    /// in production prefer running migration bundles from the deployment pipeline.
    /// </summary>
    public static async Task ApplyModuleMigrationsAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        await MigrateAsync<UsersDbContext>(scope.ServiceProvider);
        // await MigrateAsync<PayrollDbContext>(scope.ServiceProvider);
    }

    private static async Task MigrateAsync<TContext>(IServiceProvider services) where TContext : DbContext
    {
        var context = services.GetRequiredService<TContext>();
        await context.Database.MigrateAsync();
    }
}
