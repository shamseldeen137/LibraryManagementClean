using System.Text.Json;
using Confluent.Kafka;
using LibraryManagement.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace LibraryManagement.Infrastructure.Messaging;

public sealed class KafkaMessagePublisherStrategy : IMessagePublisherStrategy, IDisposable
{
    private readonly KafkaOptions _options;
    private readonly IProducer<string, string> _producer;

    public KafkaMessagePublisherStrategy(IOptions<KafkaOptions> options)
    {
        _options = options.Value;
        _producer = new ProducerBuilder<string, string>(new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            ClientId = _options.ClientId,
            Acks = Acks.All
        }).Build();
    }

    public string ProviderName => MessagingProviders.Kafka;

    public async Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default)
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = routingKey,
            Value = JsonSerializer.Serialize(message, JsonSerializerOptions.Web),
            Headers = new Headers
            {
                { "routing-key", System.Text.Encoding.UTF8.GetBytes(routingKey) }
            }
        };

        await _producer.ProduceAsync(_options.Topic, kafkaMessage, cancellationToken);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
