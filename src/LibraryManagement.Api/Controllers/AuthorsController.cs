using LibraryManagement.Application.Authors;
using LibraryManagement.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/authors")]
public sealed class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authors;

    public AuthorsController(IAuthorService authors)
    {
        _authors = authors;
    }

    [HttpGet]
    public Task<PagedResult<AuthorDto>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => _authors.GetPagedAsync(page, pageSize, cancellationToken);

    [HttpGet("{id}")]
    public Task<AuthorDto> Get(string id, CancellationToken cancellationToken)
        => _authors.GetAsync(id, cancellationToken);

    [HttpPost]
    public Task<AuthorDto> Create(CreateAuthorRequest request, CancellationToken cancellationToken)
        => _authors.CreateAsync(request, cancellationToken);

    [HttpPut("{id}")]
    public Task<AuthorDto> Update(string id, UpdateAuthorRequest request, CancellationToken cancellationToken)
        => _authors.UpdateAsync(id, request, cancellationToken);

    [HttpDelete("{id}")]
    public Task Delete(string id, CancellationToken cancellationToken)
        => _authors.DeleteAsync(id, cancellationToken);
}
