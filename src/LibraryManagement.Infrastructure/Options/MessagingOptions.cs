namespace LibraryManagement.Infrastructure.Options;

public sealed class MessagingOptions
{
    public string Provider { get; set; } = MessagingProviders.Kafka;
}

public static class MessagingProviders
{
    public const string RabbitMq = "RabbitMq";
    public const string Kafka = "Kafka";
}
