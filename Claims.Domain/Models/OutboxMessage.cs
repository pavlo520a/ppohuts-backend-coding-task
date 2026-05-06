using Claims.Domain.Enums;

namespace Claims.Domain.Models;

public sealed class OutboxMessage
{
    public required string Id { get; set; }

    public required DateTime OccurredAtUtc { get; set; }

    public required string Payload { get; set; }

    public OutboxMessageStatus Status { get; set; }

    public int Attempts { get; set; }

    public string? LastError { get; set; }

    public DateTime? SentAtUtc { get; set; }
}
