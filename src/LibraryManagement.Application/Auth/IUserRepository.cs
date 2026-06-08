namespace LibraryManagement.Application.Auth;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ApplicationUser> InsertAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}
