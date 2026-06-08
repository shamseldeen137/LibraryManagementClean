using LibraryManagement.Application.Common;
using LibraryManagement.Application.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/members")]
public sealed class MembersController : ControllerBase
{
    private readonly IMemberService _members;

    public MembersController(IMemberService members)
    {
        _members = members;
    }

    [HttpGet("names")]
    public Task<IEnumerable<string>> GetNames(CancellationToken cancellationToken = default)
        => _members.GetAllMemberNamesAsync(cancellationToken);

    [HttpGet]
    public Task<PagedResult<MemberDto>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => _members.GetPagedAsync(page, pageSize, cancellationToken);

    [HttpGet("{id}")]
    public Task<MemberDto> Get(string id, CancellationToken cancellationToken)
        => _members.GetAsync(id, cancellationToken);

    [HttpPost]
    public Task<MemberDto> Create(CreateMemberRequest request, CancellationToken cancellationToken)
        => _members.CreateAsync(request, cancellationToken);

    [HttpPut("{id}")]
    public Task<MemberDto> Update(string id, UpdateMemberRequest request, CancellationToken cancellationToken)
        => _members.UpdateAsync(id, request, cancellationToken);

    [HttpDelete("{id}")]
    public Task Delete(string id, CancellationToken cancellationToken)
        => _members.DeleteAsync(id, cancellationToken);
}
