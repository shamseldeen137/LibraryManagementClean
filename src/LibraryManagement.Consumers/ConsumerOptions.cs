namespace LibraryManagement.Consumers;

public sealed class ConsumerOptions
{
    public RabbitMqConsumerOptions RabbitMq { get; set; } = new();
    public KafkaConsumerOptions Kafka { get; set; } = new();
}

public sealed class RabbitMqConsumerOptions
{
    public string QueueName { get; set; } = "library.events.consumer";
    public string BindingKey { get; set; } = "#";
}

public sealed class KafkaConsumerOptions
{
    public string GroupId { get; set; } = "library-management-consumers";
}
