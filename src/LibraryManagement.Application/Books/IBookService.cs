using LibraryManagement.Application.Common;

namespace LibraryManagement.Application.Books;

public interface IBookService
{
    Task<PagedResult<BookDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<BookDto> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
    Task<BookDto> UpdateAsync(string id, UpdateBookRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
