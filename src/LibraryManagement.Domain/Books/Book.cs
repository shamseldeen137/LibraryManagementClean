using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Books;

public sealed class Book : Entity
{
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public int PublishedYear { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
}
