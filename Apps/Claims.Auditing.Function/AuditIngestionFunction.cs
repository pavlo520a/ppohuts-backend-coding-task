using System.Text.Json;
using Claims.Data.Auditing.Abstractions.Repositories;
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
        string auditOutbox,
        CancellationToken cancellationToken)
    {
        var outbox = JsonSerializer.Deserialize<AuditOutbox>(auditOutbox)
            ?? throw new InvalidOperationException("Invalid audit payload.");

        Func<AuditOutbox, CancellationToken, Task> handler = outbox.EntityType switch
        {
            nameof(Claim) => HandleClaimAsync,
            nameof(Cover) => HandleCoverAsync,
            _ => throw new InvalidOperationException($"Unknown aggregate type '{outbox.EntityType}'.")
        };

        await handler(outbox, cancellationToken);
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
