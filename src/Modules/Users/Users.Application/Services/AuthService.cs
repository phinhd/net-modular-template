using BuildingBlocks.Application.Exceptions;
using Users.Application.DTOs;
using Users.Application.Interfaces;
using Users.Domain.Entities;
using Users.Domain.Interfaces;

namespace Users.Application.Services;

public sealed class AuthService
{
    private readonly IUsersUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailSender _emailSender;

    public AuthService(IUsersUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _emailSender = emailSender;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = User.NormalizeEmail(request.Email);

        if (await _unitOfWork.Users.EmailExistsAsync(email, cancellationToken))
            throw new ConflictException($"Email \"{email}\" is already registered.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = User.Create(email, passwordHash, request.FullName);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Side effects run only after the user is persisted.
        await _emailSender.SendWelcomeEmailAsync(user.Email, user.FullName, cancellationToken);

        return ToResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(User.NormalizeEmail(request.Email), cancellationToken);

        // Same error for "no such user", "inactive" and "wrong password" so the API does not leak which emails exist.
        if (user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new AuthenticationFailedException();

        return ToResponse(user);
    }

    private AuthResponse ToResponse(User user)
        => new(user.Id, user.Email, user.FullName, user.Role, _jwtTokenService.GenerateToken(user));
}
