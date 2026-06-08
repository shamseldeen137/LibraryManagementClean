using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Application.Auth;

public sealed class ApplicationUser : IdentityUser<string>
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
