using LibraryManagement.Domain.Common;

namespace LibraryManagement.Domain.Loans;

public sealed class Loan : Entity
{
    public string BookId { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public DateTime LoanDateUtc { get; set; } = DateTime.UtcNow;
    public DateTime DueDateUtc { get; set; }
    public DateTime? ReturnDateUtc { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.Borrowed;
}
