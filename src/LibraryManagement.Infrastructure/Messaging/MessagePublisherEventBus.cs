using LibraryManagement.Application.Common;
using LibraryManagement.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace LibraryManagement.Infrastructure.Messaging;

public sealed class MessagePublisherEventBus : IEventBus
{
    private readonly IEnumerable<IMessagePublisherStrategy> _strategies;
    private readonly MessagingOptions _options;

    public MessagePublisherEventBus(IEnumerable<IMessagePublisherStrategy> strategies, IOptions<MessagingOptions> options)
    {
        _strategies = strategies;
        _options = options.Value;
    }

    public Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default)
    {
        var strategy = _strategies.FirstOrDefault(x =>
            string.Equals(x.ProviderName, _options.Provider, StringComparison.OrdinalIgnoreCase));

        if (strategy is null)
        {
            var supportedProviders = string.Join(", ", _strategies.Select(x => x.ProviderName));
            throw new InvalidOperationException($"Messaging provider '{_options.Provider}' is not registered. Supported providers: {supportedProviders}.");
        }

        return strategy.PublishAsync(routingKey, message, cancellationToken);
    }
}
