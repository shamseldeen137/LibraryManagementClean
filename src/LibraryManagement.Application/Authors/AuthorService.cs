using LibraryManagement.Application.Common;
using LibraryManagement.Domain.Authors;

namespace LibraryManagement.Application.Authors;

public sealed class AuthorService : IAuthorService
{
    private readonly IRepository<Author> _authors;
    private readonly ICacheService _cache;

    public AuthorService(IRepository<Author> authors, ICacheService cache)
    {
        _authors = authors;
        _cache = cache;
    }

    public async Task<PagedResult<AuthorDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _authors.GetPagedAsync(page, pageSize, cancellationToken);
        return new PagedResult<AuthorDto>(result.Items.Select(Map).ToList(), result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<AuthorDto> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"authors:{id}";
        var cached = await _cache.GetAsync<AuthorDto>(cacheKey, cancellationToken);
        if (cached is not null) return cached;

        var author = await _authors.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Author not found.");
        var dto = Map(author);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), cancellationToken);
        return dto;
    }

    public async Task<AuthorDto> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var author = await _authors.InsertAsync(new Author
        {
            Name = request.Name.Trim(),
            Biography = request.Biography,
            BirthDate = request.BirthDate
        }, cancellationToken);
        return Map(author);
    }

    public async Task<AuthorDto> UpdateAsync(string id, UpdateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        var author = await _authors.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Author not found.");
        author.Name = request.Name.Trim();
        author.Biography = request.Biography;
        author.BirthDate = request.BirthDate;
        author.UpdatedAtUtc = DateTime.UtcNow;
        await _cache.RemoveAsync($"authors:{id}", cancellationToken);
        return Map(await _authors.UpdateAsync(author, cancellationToken));
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _authors.DeleteAsync(id, cancellationToken);
        await _cache.RemoveAsync($"authors:{id}", cancellationToken);
    }

    private static AuthorDto Map(Author author) => new(author.Id, author.Name, author.Biography, author.BirthDate);
}
