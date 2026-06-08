using LibraryManagement.Application.Auth;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public Task<AuthResponse> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        return _auth.RegisterAsync(request, cancellationToken);
    }

    [HttpPost("login")]
    public Task<AuthResponse> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        return _auth.LoginAsync(request, cancellationToken);
    }
}
