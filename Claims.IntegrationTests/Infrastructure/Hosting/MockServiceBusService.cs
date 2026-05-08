using Claims.Application.Abstractions.Services;

namespace Claims.IntegrationTests.Infrastructure;

public sealed class MockServiceBusService : IServiceBusService
{
    public Task SendAsync(string payload, string messageId, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
