using System.Text;
using LibraryManagement.Infrastructure.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LibraryManagement.Consumers;

public sealed class RabbitMqLibraryEventConsumer : BackgroundService
{
    private readonly ILogger<RabbitMqLibraryEventConsumer> _logger;
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly ConsumerOptions _consumerOptions;

    public RabbitMqLibraryEventConsumer(
        ILogger<RabbitMqLibraryEventConsumer> logger,
        IOptions<RabbitMqOptions> rabbitMqOptions,
        IOptions<ConsumerOptions> consumerOptions)
    {
        _logger = logger;
        _rabbitMqOptions = rabbitMqOptions.Value;
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
                _logger.LogError(exception, "RabbitMQ consumer failed. Retrying in 5 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password
        };

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(
            exchange: _rabbitMqOptions.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _consumerOptions.RabbitMq.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: _consumerOptions.RabbitMq.QueueName,
            exchange: _rabbitMqOptions.ExchangeName,
            routingKey: _consumerOptions.RabbitMq.BindingKey,
            cancellationToken: stoppingToken);

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var payload = Encoding.UTF8.GetString(eventArgs.Body.Span);
                _logger.LogInformation(
                    "RabbitMQ event received. Exchange: {Exchange}, RoutingKey: {RoutingKey}, Payload: {Payload}",
                    eventArgs.Exchange,
                    eventArgs.RoutingKey,
                    payload);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "RabbitMQ event handling failed.");
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: _consumerOptions.RabbitMq.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation(
            "RabbitMQ consumer bound queue {QueueName} to exchange {ExchangeName} with binding key {BindingKey}.",
            _consumerOptions.RabbitMq.QueueName,
            _rabbitMqOptions.ExchangeName,
            _consumerOptions.RabbitMq.BindingKey);

        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
