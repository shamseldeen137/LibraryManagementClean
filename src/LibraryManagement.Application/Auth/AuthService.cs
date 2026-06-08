using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Application.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    public AuthService(IUserRepository users, ITokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString("N"),
            UserName = request.Email.Trim(),
            NormalizedUserName = request.Email.Trim().ToUpperInvariant(),
            Email = request.Email.Trim(),
            NormalizedEmail = request.Email.Trim().ToUpperInvariant(),
            EmailConfirmed = true,
            DisplayName = request.DisplayName.Trim(),
            SecurityStamp = Guid.NewGuid().ToString("N")
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        await _users.InsertAsync(user, cancellationToken);
        return _tokens.CreateToken(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? string.Empty, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return _tokens.CreateToken(user);
    }
}
