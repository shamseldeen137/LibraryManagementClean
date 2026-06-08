namespace LibraryManagement.Infrastructure.Messaging;

public interface IMessagePublisherStrategy
{
    string ProviderName { get; }
    Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default);
}
