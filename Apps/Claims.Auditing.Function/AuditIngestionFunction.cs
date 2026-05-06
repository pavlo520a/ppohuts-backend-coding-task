using System.Text.Json;
using Claims.Data.Auditing.Abstractions.Repositories;
using Claims.Domain.Constants;
using Claims.Domain.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Claims.Auditing.AzureFunctionIsolated;

public sealed class AuditIngestionFunction(
    IClaimAuditTrailRepository claimAuditTrailRepository,
    ICoverAuditTrailRepository coverAuditTrailRepository,
    ILogger<AuditIngestionFunction> logger)
{
    [Function("AuditIngestionFunction")]
    public async Task RunAsync(
        [ServiceBusTrigger("%ServiceBusQueueName%", Connection = "ServiceBusConnectionString")]
        string payload,
        CancellationToken cancellationToken)
    {
        var auditOutbox = JsonSerializer.Deserialize<AuditOutbox>(payload)
            ?? throw new InvalidOperationException("Invalid audit payload.");

        Func<AuditOutbox, CancellationToken, Task> handler = auditOutbox.EntityType switch
        {
            AuditAggregateTypes.Claim => HandleClaimAsync,
            AuditAggregateTypes.Cover => HandleCoverAsync,
            _ => throw new InvalidOperationException($"Unknown aggregate type '{auditOutbox.EntityType}'.")
        };

        await handler(auditOutbox, cancellationToken);
    }

    private async Task HandleClaimAsync(AuditOutbox auditOutbox, CancellationToken cancellationToken)
    {
        var exists = await claimAuditTrailRepository.AnyAsync(
            auditOutbox.EntityId,
            auditOutbox.HttpMethod,
            cancellationToken);

        if (exists)
        {
            logger.LogInformation("Duplicate claim audit ignored for entity {EntityId}.", auditOutbox.EntityId);
            return;
        }

        await claimAuditTrailRepository.AddAsync(auditOutbox.EntityId, auditOutbox.HttpMethod, cancellationToken);
    }

    private async Task HandleCoverAsync(AuditOutbox auditOutbox, CancellationToken cancellationToken)
    {
        var exists = await coverAuditTrailRepository.AnyAsync(
            auditOutbox.EntityId,
            auditOutbox.HttpMethod,
            cancellationToken);

        if (exists)
        {
            logger.LogInformation("Duplicate cover audit ignored for entity {EntityId}.", auditOutbox.EntityId);
            return;
        }

        await coverAuditTrailRepository.AddAsync(auditOutbox.EntityId, auditOutbox.HttpMethod, cancellationToken);
    }
}
