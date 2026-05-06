using Azure.Messaging.ServiceBus;
using Claims.Application.Options.Outbox;
using Microsoft.Extensions.Options;

namespace Claims.Application.Services;

public sealed class ServiceBusService : IServiceBusService, IAsyncDisposable
{
    private readonly ServiceBusClient client;
    private readonly ServiceBusSender sender;

    public ServiceBusService(IOptions<ServiceBusOptions> options)
    {
        client = new ServiceBusClient(options.Value.ConnectionString);
        sender = client.CreateSender(options.Value.QueueName);
    }

    public async Task SendAsync(string payload, string messageId, string subject, CancellationToken cancellationToken)
    {
        var message = new ServiceBusMessage(payload)
        {
            MessageId = messageId,
            Subject = subject
        };

        await sender.SendMessageAsync(message, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await sender.DisposeAsync();
        await client.DisposeAsync();
    }
}
