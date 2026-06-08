using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.Authors;

public sealed record AuthorDto(string Id, string Name, string? Biography, DateTime? BirthDate);

public sealed record CreateAuthorRequest(
    [Required, MaxLength(128)] string Name,
    [MaxLength(2048)] string? Biography,
    DateTime? BirthDate);

public sealed record UpdateAuthorRequest(
    [Required, MaxLength(128)] string Name,
    [MaxLength(2048)] string? Biography,
    DateTime? BirthDate);
