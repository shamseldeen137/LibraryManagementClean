using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Authors;

public sealed class Author : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
}
