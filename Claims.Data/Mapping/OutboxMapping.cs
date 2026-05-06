using Claims.Data.Documents;
using Claims.Domain.Models;

namespace Claims.Data.Mapping;

public static class OutboxMapping
{
    public static OutboxMessage ToDomain(this OutboxMessageDocument document) =>
        new()
        {
            Id = document.Id,
            OccurredAtUtc = document.OccurredAtUtc,
            Payload = document.Payload,
            Status = document.Status,
            Attempts = document.Attempts,
            LastError = document.LastError,
            SentAtUtc = document.SentAtUtc
        };
}
