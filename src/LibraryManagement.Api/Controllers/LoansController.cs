using LibraryManagement.Application.Common;
using LibraryManagement.Application.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/loans")]
public sealed class LoansController : ControllerBase
{
    private readonly ILoanService _loans;

    public LoansController(ILoanService loans)
    {
        _loans = loans;
    }

    [HttpGet]
    public Task<PagedResult<LoanDto>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        => _loans.GetPagedAsync(page, pageSize, cancellationToken);

    [HttpGet("{id}")]
    public Task<LoanDto> Get(string id, CancellationToken cancellationToken)
        => _loans.GetAsync(id, cancellationToken);

    [HttpPost]
    public Task<LoanDto> Create(CreateLoanRequest request, CancellationToken cancellationToken)
        => _loans.CreateAsync(request, cancellationToken);

    [HttpPost("{id}/return")]
    public Task<LoanDto> Return(string id, CancellationToken cancellationToken)
        => _loans.ReturnAsync(id, cancellationToken);

    [HttpDelete("{id}")]
    public Task Delete(string id, CancellationToken cancellationToken)
        => _loans.DeleteAsync(id, cancellationToken);
}
