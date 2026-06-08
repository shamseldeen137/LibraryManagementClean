using LibraryManagement.Application.Common;
using LibraryManagement.Domain.Books;
using LibraryManagement.Domain.Loans;

namespace LibraryManagement.Application.Loans;

public sealed class LoanService : ILoanService
{
    private readonly IRepository<Loan> _loans;
    private readonly IRepository<Book> _books;
    private readonly IEventBus _eventBus;

    public LoanService(IRepository<Loan> loans, IRepository<Book> books, IEventBus eventBus)
    {
        _loans = loans;
        _books = books;
        _eventBus = eventBus;
    }

    public async Task<PagedResult<LoanDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _loans.GetPagedAsync(page, pageSize, cancellationToken);
        return new PagedResult<LoanDto>(result.Items.Select(Map).ToList(), result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<LoanDto> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var loan = await _loans.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Loan not found.");
        return Map(loan);
    }

    public async Task<LoanDto> CreateAsync(CreateLoanRequest request, CancellationToken cancellationToken = default)
    {
        var book = await _books.GetByIdAsync(request.BookId, cancellationToken) ?? throw new KeyNotFoundException("Book not found.");
        if (book.AvailableCopies <= 0) throw new InvalidOperationException("Book has no available copies.");

        book.AvailableCopies--;
        book.UpdatedAtUtc = DateTime.UtcNow;
        await _books.UpdateAsync(book, cancellationToken);

        var loan = await _loans.InsertAsync(new Loan
        {
            BookId = request.BookId,
            MemberId = request.MemberId,
            DueDateUtc = request.DueDateUtc
        }, cancellationToken);

        await _eventBus.PublishAsync("library.loan.created", new { loan.Id, loan.BookId, loan.MemberId }, cancellationToken);
        return Map(loan);
    }

    public async Task<LoanDto> ReturnAsync(string id, CancellationToken cancellationToken = default)
    {
        var loan = await _loans.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Loan not found.");
        if (loan.Status == LoanStatus.Returned) return Map(loan);

        loan.Status = LoanStatus.Returned;
        loan.ReturnDateUtc = DateTime.UtcNow;
        loan.UpdatedAtUtc = DateTime.UtcNow;

        var book = await _books.GetByIdAsync(loan.BookId, cancellationToken);
        if (book is not null && book.AvailableCopies < book.TotalCopies)
        {
            book.AvailableCopies++;
            book.UpdatedAtUtc = DateTime.UtcNow;
            await _books.UpdateAsync(book, cancellationToken);
        }

        var updated = await _loans.UpdateAsync(loan, cancellationToken);
        await _eventBus.PublishAsync("library.loan.returned", new { loan.Id, loan.BookId, loan.MemberId }, cancellationToken);
        return Map(updated);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default) => _loans.DeleteAsync(id, cancellationToken);

    private static LoanDto Map(Loan loan) => new(loan.Id, loan.BookId, loan.MemberId, loan.LoanDateUtc, loan.DueDateUtc, loan.ReturnDateUtc, loan.Status);
}
