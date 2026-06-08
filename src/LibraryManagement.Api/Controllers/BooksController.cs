using LibraryManagement.Application.Books;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.ExternalCatalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/books")]
public sealed class BooksController : ControllerBase
{
    private readonly IBookService _books;
    private readonly IExternalBookCatalog _externalBookCatalog;

    public BooksController(IBookService books, IExternalBookCatalog externalBookCatalog)
    {
        _books = books;
        _externalBookCatalog = externalBookCatalog;
    }

    [HttpGet]
    public Task<PagedResult<BookDto>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => _books.GetPagedAsync(page, pageSize, cancellationToken);

    [HttpGet("{id}")]
    public Task<BookDto> Get(string id, CancellationToken cancellationToken)
        => _books.GetAsync(id, cancellationToken);

    [HttpGet("external/{isbn}")]
    public async Task<ActionResult<ExternalBookCatalogDto>> GetExternalByIsbn(string isbn, CancellationToken cancellationToken)
    {
        var book = await _externalBookCatalog.GetByIsbnAsync(isbn, cancellationToken);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public Task<BookDto> Create(CreateBookRequest request, CancellationToken cancellationToken)
        => _books.CreateAsync(request, cancellationToken);

    [HttpPut("{id}")]
    public Task<BookDto> Update(string id, UpdateBookRequest request, CancellationToken cancellationToken)
        => _books.UpdateAsync(id, request, cancellationToken);

    [HttpDelete("{id}")]
    public Task Delete(string id, CancellationToken cancellationToken)
        => _books.DeleteAsync(id, cancellationToken);
}
