using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LibraryManagement.Application.ExternalCatalog;

namespace LibraryManagement.Infrastructure.ExternalCatalog;

public sealed class OpenLibraryBookCatalogAdapter : IExternalBookCatalog
{
    private readonly HttpClient _httpClient;

    public OpenLibraryBookCatalogAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExternalBookCatalogDto?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var normalizedIsbn = NormalizeIsbn(isbn);
        if (string.IsNullOrWhiteSpace(normalizedIsbn))
        {
            throw new ArgumentException("ISBN is required.", nameof(isbn));
        }

        using var response = await _httpClient.GetAsync($"/isbn/{Uri.EscapeDataString(normalizedIsbn)}.json", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var book = await response.Content.ReadFromJsonAsync<OpenLibraryBookResponse>(cancellationToken);
        if (book is null)
        {
            return null;
        }

        return new ExternalBookCatalogDto(
            normalizedIsbn,
            book.Title,
            book.AuthorKeys,
            ExtractPublishedYear(book.PublishDate),
            $"/isbn/{normalizedIsbn}");
    }

    private static string NormalizeIsbn(string isbn)
        => new(isbn.Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant).ToArray());

    private static int? ExtractPublishedYear(string? publishDate)
    {
        if (string.IsNullOrWhiteSpace(publishDate))
        {
            return null;
        }

        var yearText = new string(publishDate.Where(char.IsDigit).TakeLast(4).ToArray());
        return int.TryParse(yearText, NumberStyles.None, CultureInfo.InvariantCulture, out var year)
            ? year
            : null;
    }

    private sealed record OpenLibraryBookResponse(
        [property: JsonPropertyName("title")] string? Title,
        [property: JsonPropertyName("publish_date")] string? PublishDate,
        [property: JsonPropertyName("authors")] IReadOnlyCollection<OpenLibraryAuthorReference>? Authors)
    {
        public IReadOnlyCollection<string> AuthorKeys => Authors?.Select(author => author.Key).ToArray() ?? [];
    }

    private sealed record OpenLibraryAuthorReference([property: JsonPropertyName("key")] string Key);
}
