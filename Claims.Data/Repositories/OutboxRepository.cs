using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Enums;
using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Repositories;

public sealed class OutboxRepository(ClaimsMongoDbContext context) : IOutboxRepository
{
    public void Add(AuditOutbox auditOutbox)
    {
        context.OutboxMessages.Add(new Documents.OutboxMessageDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            OccurredAtUtc = auditOutbox.OccurredAtUtc,
            AggregateType = auditOutbox.EntityType,
            AggregateId = auditOutbox.EntityId,
            Operation = "audit",
            HttpMethod = auditOutbox.HttpMethod,
            Payload = System.Text.Json.JsonSerializer.Serialize(auditOutbox),
            Status = OutboxMessageStatus.Pending,
            Attempts = 0
        });
    }

    public async Task MarkProcessingAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await context.OutboxMessages.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return;
        }

        entity.Status = OutboxMessageStatus.Processing;
        entity.LastError = null;
    }

    public async Task MarkSentAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await context.OutboxMessages.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        if (entity is null)
        {
            return;
        }

        entity.Status = OutboxMessageStatus.Sent;
        entity.SentAtUtc = DateTime.UtcNow;
        entity.LastError = null;
    }

    public async Task MarkFailedAsync(string id, string error, CancellationToken cancellationToken)
    {
        var entity = await context.OutboxMessages.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        if (entity is null)
        {
            return;
        }

        entity.Status = OutboxMessageStatus.Pending;
        entity.Attempts += 1;
        entity.LastError = error;
    }
}
