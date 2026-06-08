namespace LibraryManagement.Application.Auth;

public interface ITokenService
{
    AuthResponse CreateToken(ApplicationUser user);
}
