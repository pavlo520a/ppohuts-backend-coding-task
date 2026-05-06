using Claims.Application.Abstractions.Services;
using Claims.Application.Options.Outbox;
using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Queries;
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
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var outboxQuery = scope.ServiceProvider.GetRequiredService<IOutboxQuery>();
                var batch = await outboxQuery.GetPendingBatchAsync(processorOptions.Value.BatchSize, stoppingToken);

                foreach (var entry in batch)
                {
                    await unitOfWork.OutboxRepository.MarkProcessingAsync(entry.Id, stoppingToken);
                    await unitOfWork.SaveChangesAsync(stoppingToken);

                    try
                    {
                        await serviceBusService.SendAsync(entry.Payload, entry.Id, entry.AggregateType, stoppingToken);
                        await unitOfWork.OutboxRepository.MarkSentAsync(entry.Id, stoppingToken);
                        await unitOfWork.SaveChangesAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to send outbox message {MessageId}", entry.Id);
                        await unitOfWork.OutboxRepository.MarkFailedAsync(entry.Id, ex.Message, stoppingToken);
                        await unitOfWork.SaveChangesAsync(stoppingToken);
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
