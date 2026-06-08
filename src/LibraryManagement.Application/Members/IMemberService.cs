using LibraryManagement.Application.Common;

namespace LibraryManagement.Application.Members;

public interface IMemberService
{
    Task<IEnumerable<string>> GetAllMemberNamesAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<MemberDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<MemberDto> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<MemberDto> CreateAsync(CreateMemberRequest request, CancellationToken cancellationToken = default);
    Task<MemberDto> UpdateAsync(string id, UpdateMemberRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
