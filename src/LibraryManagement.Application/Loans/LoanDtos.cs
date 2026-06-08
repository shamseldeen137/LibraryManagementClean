using System.ComponentModel.DataAnnotations;
using LibraryManagement.Domain.Loans;

namespace LibraryManagement.Application.Loans;

public sealed record LoanDto(
    string Id,
    string BookId,
    string MemberId,
    DateTime LoanDateUtc,
    DateTime DueDateUtc,
    DateTime? ReturnDateUtc,
    LoanStatus Status);

public sealed record CreateLoanRequest([Required] string BookId, [Required] string MemberId, [Required] DateTime DueDateUtc);
