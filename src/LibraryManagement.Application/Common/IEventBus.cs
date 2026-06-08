namespace LibraryManagement.Application.Common;

public interface IEventBus
{
    Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default);
}
