using LibraryManagement.Application.Common;

namespace LibraryManagement.Application.Loans;

public interface ILoanService
{
    Task<PagedResult<LoanDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<LoanDto> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<LoanDto> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default);
    Task<LoanDto> ReturnAsync(string id, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
