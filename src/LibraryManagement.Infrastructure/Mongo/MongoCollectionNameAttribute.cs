namespace LibraryManagement.Infrastructure.Mongo;

[AttributeUsage(AttributeTargets.Class)]
public sealed class MongoCollectionNameAttribute : Attribute
{
    public MongoCollectionNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
