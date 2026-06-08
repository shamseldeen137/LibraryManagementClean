using LibraryManagement.Application.Common;

namespace LibraryManagement.Application.Authors;

public interface IAuthorService
{
    Task<PagedResult<AuthorDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<AuthorDto> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<AuthorDto> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<AuthorDto> UpdateAsync(string id, UpdateAuthorRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
