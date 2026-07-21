using BuildingBlocks.Application.Exceptions;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Interfaces;

namespace Users.Application.Services;

public class AuthService
{
    private readonly IUsersUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IUsersUnitOfWork unitOfWork, IJwtTokenService jwtTokenService)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
            throw new BusinessRuleException($"Email \"{request.Email}\" is already registered.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = User.Create(request.Email, passwordHash, request.FullName);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResponse(user.Id, user.Email, user.FullName, user.Role, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new AuthenticationFailedException();

        if (!user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new AuthenticationFailedException();

        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResponse(user.Id, user.Email, user.FullName, user.Role, token);
    }
}
