using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.Books;

public sealed record BookDto(string Id, string Title, string Isbn, string AuthorId, int PublishedYear, int TotalCopies, int AvailableCopies);

public sealed record CreateBookRequest(
    [Required, MaxLength(256)] string Title,
    [Required, MaxLength(32)] string Isbn,
    [Required] string AuthorId,
    [Range(1, 9999)] int PublishedYear,
    [Range(0, int.MaxValue)] int TotalCopies);

public sealed record UpdateBookRequest(
    [Required, MaxLength(256)] string Title,
    [Required, MaxLength(32)] string Isbn,
    [Required] string AuthorId,
    [Range(1, 9999)] int PublishedYear,
    [Range(0, int.MaxValue)] int TotalCopies,
    [Range(0, int.MaxValue)] int AvailableCopies);
