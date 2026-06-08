using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.Members;

public sealed record MemberDto(string Id, string FullName, string Email, string? PhoneNumber, DateTime MembershipDateUtc, bool IsActive);

public sealed record CreateMemberRequest(
    [Required, MaxLength(128)] string FullName,
    [Required, EmailAddress, MaxLength(256)] string Email,
    [MaxLength(32)] string? PhoneNumber);

public sealed record UpdateMemberRequest(
    [Required, MaxLength(128)] string FullName,
    [Required, EmailAddress, MaxLength(256)] string Email,
    [MaxLength(32)] string? PhoneNumber,
    bool IsActive);
