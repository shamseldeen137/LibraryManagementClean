namespace LibraryManagement.Application.ExternalCatalog;

public sealed record ExternalBookCatalogDto(
    string Isbn,
    string? Title,
    IReadOnlyCollection<string> Authors,
    int? PublishedYear,
    string? SourceUrl);
