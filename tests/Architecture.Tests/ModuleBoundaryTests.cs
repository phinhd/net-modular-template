using NetArchTest.Rules;

namespace Architecture.Tests;

/// <summary>
/// Guards the rules the template is built around. If someone adds a forbidden
/// reference, CI fails here instead of the architecture eroding silently.
/// </summary>
public sealed class ModuleBoundaryTests
{
    private static readonly System.Reflection.Assembly UsersDomain = typeof(Users.Domain.Entities.User).Assembly;
    private static readonly System.Reflection.Assembly UsersApplication = typeof(Users.Application.Services.AuthService).Assembly;
    private static readonly System.Reflection.Assembly UsersInfrastructure = typeof(Users.Infrastructure.DependencyInjection).Assembly;
    private static readonly System.Reflection.Assembly BuildingBlocksDomain = typeof(BuildingBlocks.Domain.Common.BaseEntity).Assembly;

    [Fact]
    public void Domain_DoesNotDependOn_ApplicationOrInfrastructure()
    {
        var result = Types.InAssembly(UsersDomain)
            .ShouldNot()
            .HaveDependencyOnAny("Users.Application", "Users.Infrastructure", "Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_DoesNotDependOn_Infrastructure()
    {
        var result = Types.InAssembly(UsersApplication)
            .ShouldNot()
            .HaveDependencyOnAny("Users.Infrastructure", "Microsoft.EntityFrameworkCore", "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void BuildingBlocksDomain_HasNoFrameworkDependencies()
    {
        var result = Types.InAssembly(BuildingBlocksDomain)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Modules_DoNotReferenceTheHost()
    {
        var result = Types.InAssemblies([UsersDomain, UsersApplication, UsersInfrastructure])
            .ShouldNot()
            .HaveDependencyOn("WebApi")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    private static string Describe(TestResult result)
        => "Violating types: " + string.Join(", ", result.FailingTypeNames ?? []);
}
