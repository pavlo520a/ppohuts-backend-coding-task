using Claims.Application.Abstractions.Services;
using Claims.Application.Options.Outbox;
using Claims.Data.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Claims.Application.HostedServices;

public sealed class OutboxQueueHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxProcessorOptions> processorOptions,
    IServiceBusService serviceBusService,
    ILogger<OutboxQueueHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var batch = await outboxRepository.GetPendingBatchAsync(processorOptions.Value.BatchSize, stoppingToken);

                foreach (var entry in batch)
                {
                    await outboxRepository.MarkProcessingAsync(entry.Id, stoppingToken);

                    try
                    {
                        await serviceBusService.SendAsync(entry.Payload, entry.Id, entry.AggregateType, stoppingToken);
                        await outboxRepository.MarkSentAsync(entry.Id, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to send outbox message {MessageId}", entry.Id);
                        await outboxRepository.MarkFailedAsync(entry.Id, ex.Message, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox batch iteration failed.");
            }

            await Task.Delay(processorOptions.Value.PollIntervalMs, stoppingToken);
        }
    }
}
