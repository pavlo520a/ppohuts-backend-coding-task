using Claims.Application.Abstractions.Services;
using Claims.Application.Options.Outbox;
using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Queries;
using Claims.Domain.Models;
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
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var outboxQuery = scope.ServiceProvider.GetRequiredService<IOutboxQuery>();

                var batch = await outboxQuery.GetPendingBatchAsync(
                    processorOptions.Value.BatchSize,
                    cancellationToken);

                foreach (var entry in batch)
                {
                    await unitOfWork.OutboxRepository.MarkProcessingAsync(entry.Id, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    try
                    {
                        await serviceBusService.SendAsync(
                            entry.Payload,
                            entry.Id,
                            cancellationToken);

                        await unitOfWork.OutboxRepository.MarkSentAsync(entry.Id, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to send outbox message {MessageId}", entry.Id);

                        await HandleSendFailureAsync(
                            unitOfWork,
                            entry,
                            ex.Message,
                            cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox batch iteration failed.");
            }

            await Task.Delay(processorOptions.Value.PollIntervalMs, cancellationToken);
        }
    }

    private async Task HandleSendFailureAsync(
        IUnitOfWork unitOfWork,
        OutboxMessage entry,
        string error,
        CancellationToken cancellationToken)
    {
        if (entry.Attempts + 1 >= processorOptions.Value.MaxAttempts)
        {
            await unitOfWork.OutboxRepository.MarkFailedAsync(entry.Id, cancellationToken);
        }
        else
        {
            await unitOfWork.OutboxRepository.RegisterAttemptAsync(entry.Id, error, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
