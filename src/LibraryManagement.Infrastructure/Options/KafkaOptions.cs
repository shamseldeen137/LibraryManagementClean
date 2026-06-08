namespace LibraryManagement.Infrastructure.Options;

public sealed class KafkaOptions
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = "library-events";
    public string ClientId { get; set; } = "library-management-api";
}
