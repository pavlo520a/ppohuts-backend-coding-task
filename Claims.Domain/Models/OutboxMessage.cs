using Claims.Domain.Enums;

namespace Claims.Domain.Models;

public sealed class OutboxMessage
{
    public required string Id { get; init; }

    public required DateTime OccurredAtUtc { get; init; }

    public required string AggregateType { get; init; }

    public required string AggregateId { get; init; }

    public required string Operation { get; init; }

    public required string HttpMethod { get; init; }

    public required string Payload { get; init; }

    public OutboxMessageStatus Status { get; init; }

    public int Attempts { get; init; }

    public string? LastError { get; init; }

    public DateTime? SentAtUtc { get; init; }
}
