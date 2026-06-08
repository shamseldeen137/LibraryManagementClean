using LibraryManagement.Application.Auth;
using LibraryManagement.Infrastructure.Mongo;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Auth;

public sealed class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<ApplicationUser> _users;

    public MongoUserRepository(MongoDbContext context)
    {
        _users = context.Database.GetCollection<ApplicationUser>("users");
    }

    public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToUpperInvariant();
        return _users.Find(x => x.NormalizedEmail == normalized).FirstOrDefaultAsync(cancellationToken)!;
    }

    public Task<ApplicationUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _users.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken)!;
    }

    public async Task<ApplicationUser> InsertAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        await _users.InsertOneAsync(user, cancellationToken: cancellationToken);
        return user;
    }

    public Task UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        return _users.ReplaceOneAsync(x => x.Id == user.Id, user, cancellationToken: cancellationToken);
    }
}
