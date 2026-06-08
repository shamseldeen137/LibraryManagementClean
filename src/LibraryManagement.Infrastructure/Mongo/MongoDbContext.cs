using LibraryManagement.Infrastructure.Options;
using LibraryManagement.Application.Auth;
using LibraryManagement.Domain.Authors;
using LibraryManagement.Domain.Books;
using LibraryManagement.Domain.Loans;
using LibraryManagement.Domain.Members;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LibraryManagement.Infrastructure.Mongo;

public sealed class MongoDbContext
{
    public MongoDbContext(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        Database = client.GetDatabase(options.Value.DatabaseName);
    }

    public IMongoDatabase Database { get; }

    public IMongoCollection<T> Collection<T>()
    {
        var name = typeof(T) switch
        {
            var type when type == typeof(Author) => "authors",
            var type when type == typeof(Book) => "books",
            var type when type == typeof(Member) => "members",
            var type when type == typeof(Loan) => "loans",
            var type when type == typeof(ApplicationUser) => "users",
            _ => typeof(T).Name
        };

        return Database.GetCollection<T>(name);
    }
}
