using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Users.Application.DTOs;

namespace WebApi.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class AuthEndpointsTests
{
    private const string Password = "Password123!";
    private readonly HttpClient _client;

    public AuthEndpointsTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Then_Login_Then_GetMe_Works()
    {
        var email = UniqueEmail();

        var register = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, Password, "Integration User"));
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);

        var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email.ToUpperInvariant(), Password));
        login.EnsureSuccessStatusCode();
        var auth = await login.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
        var me = await _client.SendAsync(request);

        me.EnsureSuccessStatusCode();
        var profile = await me.Content.ReadFromJsonAsync<UserDto>();
        Assert.Equal(email, profile!.Email);
        Assert.Equal("Integration User", profile.FullName);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409Problem()
    {
        var email = UniqueEmail();
        (await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, Password, "First"))).EnsureSuccessStatusCode();

        var duplicate = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, Password, "Second"));

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        var problem = await duplicate.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal(409, problem!.Status);
    }

    [Fact]
    public async Task Register_InvalidPayload_Returns400ValidationProblem()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest("not-an-email", "short", ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("Email", problem!.Errors.Keys);
        Assert.Contains("Password", problem.Errors.Keys);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var email = UniqueEmail();
        (await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, Password, "User"))).EnsureSuccessStatusCode();

        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "WrongPassword!"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Health_ReportsHealthy_WhenDatabaseIsReachable()
    {
        var response = await _client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    private static string UniqueEmail() => $"user-{Guid.NewGuid():N}@example.com";
}
