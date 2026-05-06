using Claims.Data.Abstractions.Outbox;
using Claims.Data.Documents;

namespace Claims.Data.Mapping;

public static class OutboxMapping
{
    public static OutboxMessage ToDomain(this OutboxMessageDocument document) =>
        new()
        {
            Id = document.Id,
            OccurredAtUtc = document.OccurredAtUtc,
            AggregateType = document.AggregateType,
            AggregateId = document.AggregateId,
            Operation = document.Operation,
            HttpMethod = document.HttpMethod,
            Payload = document.Payload,
            Status = document.Status,
            Attempts = document.Attempts,
            LastError = document.LastError,
            SentAtUtc = document.SentAtUtc
        };
}
