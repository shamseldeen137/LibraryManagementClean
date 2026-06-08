using LibraryManagement.Application.Auth;
using LibraryManagement.Domain.Authors;
using LibraryManagement.Domain.Books;
using LibraryManagement.Domain.Loans;
using LibraryManagement.Domain.Members;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace LibraryManagement.Infrastructure.Mongo;

public static class MongoConventions
{
    private static bool _configured;

    public static void Configure()
    {
        if (_configured) return;
        _configured = true;

        BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
        Register<Author>("authors");
        Register<Book>("books");
        Register<Member>("members");
        Register<Loan>("loans");
        Register<ApplicationUser>("users");
    }

    private static void Register<T>(string collection)
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
        {
            BsonClassMap.RegisterClassMap<T>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
        }
    }
}
