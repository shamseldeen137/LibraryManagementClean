using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Members;

public sealed class Member : Entity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime MembershipDateUtc { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
