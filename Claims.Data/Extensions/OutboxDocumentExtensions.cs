using System.Text.Json;
using Claims.Data.Documents;
using Claims.Domain.Enums;
using Claims.Domain.Models;

namespace Claims.Data.Extensions;

internal static class OutboxDocumentExtensions
{
    public static OutboxMessageDocument ToOutboxMessage(this BaseDocument document, string aggregateType, string httpMethod)
    {
        var occurredAtUtc = DateTime.UtcNow;
        var payload = new AuditOutbox
        {
            EntityType = aggregateType,
            EntityId = document.Id,
            HttpMethod = httpMethod,
            OccurredAtUtc = occurredAtUtc
        };

        return new OutboxMessageDocument
        {
            Id = Guid.NewGuid().ToString("N"),
            OccurredAtUtc = occurredAtUtc,
            AggregateType = aggregateType,
            AggregateId = document.Id,
            Operation = "audit",
            HttpMethod = httpMethod,
            Payload = JsonSerializer.Serialize(payload),
            Status = OutboxMessageStatus.Pending,
            Attempts = 0
        };
    }
}
