using LibraryManagement.Application.Common;
using LibraryManagement.Domain.Members;

namespace LibraryManagement.Application.Members;

public sealed class MemberService : IMemberService
{
    private readonly IRepository<Member> _members;
    private readonly ICacheService _cache;

    public MemberService(IRepository<Member> members, ICacheService cache)
    {
        _members = members;
        _cache = cache;
    }

    public async Task<IEnumerable<string>> GetAllMemberNamesAsync(CancellationToken cancellationToken = default)
    {
        var members = await _members.GetAllAsync(cancellationToken);
        return members.Select(x => x.FullName);
    }

    public async Task<PagedResult<MemberDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _members.GetPagedAsync(page, pageSize, cancellationToken);
        return new PagedResult<MemberDto>(result.Items.Select(Map).ToList(), result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<MemberDto> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"members:{id}";
        var cached = await _cache.GetAsync<MemberDto>(cacheKey, cancellationToken);
        if (cached is not null) return cached;

        var member = await _members.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Member not found.");
        var dto = Map(member);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), cancellationToken);
        return dto;
    }

    public async Task<MemberDto> CreateAsync(CreateMemberRequest request, CancellationToken cancellationToken = default)
    {
        var member = await _members.InsertAsync(new Member
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber
        }, cancellationToken);
        return Map(member);
    }

    public async Task<MemberDto> UpdateAsync(string id, UpdateMemberRequest request, CancellationToken cancellationToken = default)
    {
        var member = await _members.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Member not found.");
        member.FullName = request.FullName.Trim();
        member.Email = request.Email.Trim();
        member.PhoneNumber = request.PhoneNumber;
        member.IsActive = request.IsActive;
        member.UpdatedAtUtc = DateTime.UtcNow;
        await _cache.RemoveAsync($"members:{id}", cancellationToken);
        return Map(await _members.UpdateAsync(member, cancellationToken));
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _members.DeleteAsync(id, cancellationToken);
        await _cache.RemoveAsync($"members:{id}", cancellationToken);
    }

    private static MemberDto Map(Member member) => new(member.Id, member.FullName, member.Email, member.PhoneNumber, member.MembershipDateUtc, member.IsActive);
}
