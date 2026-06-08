using System.Text;
using Confluent.Kafka;
using LibraryManagement.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace LibraryManagement.Consumers;

public sealed class KafkaLibraryEventConsumer : BackgroundService
{
    private readonly ILogger<KafkaLibraryEventConsumer> _logger;
    private readonly KafkaOptions _kafkaOptions;
    private readonly ConsumerOptions _consumerOptions;

    public KafkaLibraryEventConsumer(
        ILogger<KafkaLibraryEventConsumer> logger,
        IOptions<KafkaOptions> kafkaOptions,
        IOptions<ConsumerOptions> consumerOptions)
    {
        _logger = logger;
        _kafkaOptions = kafkaOptions.Value;
        _consumerOptions = consumerOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Kafka consumer failed. Retrying in 5 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private Task ConsumeAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _consumerOptions.Kafka.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            ClientId = $"{_kafkaOptions.ClientId}-consumer"
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_kafkaOptions.Topic);
        _logger.LogInformation("Kafka consumer subscribed to topic {Topic}.", _kafkaOptions.Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = consumer.Consume(stoppingToken);
            var routingKey = GetRoutingKey(result.Message);

            _logger.LogInformation(
                "Kafka event received. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, RoutingKey: {RoutingKey}, Payload: {Payload}",
                result.Topic,
                result.Partition.Value,
                result.Offset.Value,
                routingKey,
                result.Message.Value);

            consumer.Commit(result);
        }

        return Task.CompletedTask;
    }

    private static string GetRoutingKey(Message<string, string> message)
    {
        if (!string.IsNullOrWhiteSpace(message.Key))
        {
            return message.Key;
        }

        var header = message.Headers?.FirstOrDefault(header => header.Key == "routing-key");
        return header is null ? string.Empty : Encoding.UTF8.GetString(header.GetValueBytes());
    }
}
