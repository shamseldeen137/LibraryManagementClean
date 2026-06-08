using LibraryManagement.Application.Auth;
using LibraryManagement.Application.Authors;
using LibraryManagement.Application.Books;
using LibraryManagement.Application.Loans;
using LibraryManagement.Application.Members;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<ILoanService, LoanService>();
        return services;
    }
}
