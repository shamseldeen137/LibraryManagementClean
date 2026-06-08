using System.Text;
using System.Text.Json;
using LibraryManagement.Application.Common;
using LibraryManagement.Infrastructure.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace LibraryManagement.Infrastructure.Messaging;

public sealed class RabbitMqMessagePublisherStrategy : IMessagePublisherStrategy, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqMessagePublisherStrategy(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public string ProviderName => MessagingProviders.RabbitMq;

    public async Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default)
    {
        var channel = await GetChannelAsync(cancellationToken);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, JsonSerializerOptions.Web));

        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: new BasicProperties { ContentType = "application/json", DeliveryMode = DeliveryModes.Persistent },
            body: body,
            cancellationToken: cancellationToken);
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) return _channel;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.ExchangeDeclareAsync(_options.ExchangeName, ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);
        return _channel;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
    }
}
