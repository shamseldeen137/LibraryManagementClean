using LibraryManagement.Application.Common;
using LibraryManagement.Domain.Books;

namespace LibraryManagement.Application.Books;

public sealed class BookService : IBookService
{
    private readonly IRepository<Book> _books;
    private readonly ICacheService _cache;

    public BookService(IRepository<Book> books, ICacheService cache)
    {
        _books = books;
        _cache = cache;
    }

    public async Task<PagedResult<BookDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _books.GetPagedAsync(page, pageSize, cancellationToken);
        return new PagedResult<BookDto>(result.Items.Select(Map).ToList(), result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<BookDto> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"books:{id}";
        var cached = await _cache.GetAsync<BookDto>(cacheKey, cancellationToken);
        if (cached is not null) return cached;

        var book = await _books.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Book not found.");
        var dto = Map(book);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), cancellationToken);
        return dto;
    }

    public async Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        var book = await _books.InsertAsync(new Book
        {
            Title = request.Title.Trim(),
            Isbn = request.Isbn.Trim(),
            AuthorId = request.AuthorId,
            PublishedYear = request.PublishedYear,
            TotalCopies = request.TotalCopies,
            AvailableCopies = request.TotalCopies
        }, cancellationToken);
        return Map(book);
    }

    public async Task<BookDto> UpdateAsync(string id, UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        var book = await _books.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Book not found.");
        book.Title = request.Title.Trim();
        book.Isbn = request.Isbn.Trim();
        book.AuthorId = request.AuthorId;
        book.PublishedYear = request.PublishedYear;
        book.TotalCopies = request.TotalCopies;
        book.AvailableCopies = request.AvailableCopies;
        book.UpdatedAtUtc = DateTime.UtcNow;
        await _cache.RemoveAsync($"books:{id}", cancellationToken);
        return Map(await _books.UpdateAsync(book, cancellationToken));
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _books.DeleteAsync(id, cancellationToken);
        await _cache.RemoveAsync($"books:{id}", cancellationToken);
    }

    private static BookDto Map(Book book) => new(book.Id, book.Title, book.Isbn, book.AuthorId, book.PublishedYear, book.TotalCopies, book.AvailableCopies);
}
