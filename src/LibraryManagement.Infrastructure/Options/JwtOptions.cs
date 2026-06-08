namespace LibraryManagement.Infrastructure.Options;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "LibraryManagementClean";
    public string Audience { get; set; } = "LibraryManagementClean";
    public string SigningKey { get; set; } = "CHANGE_ME_TO_A_LONG_SECRET_KEY_32_CHARS_MIN";
    public int ExpirationMinutes { get; set; } = 120;
}
