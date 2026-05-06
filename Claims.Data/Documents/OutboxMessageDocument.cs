using Claims.Data.Abstractions.Outbox;

namespace Claims.Data.Documents;

public sealed class OutboxMessageDocument : BaseDocument
{
    public DateTime OccurredAtUtc { get; set; }

    public required string AggregateType { get; set; }

    public required string AggregateId { get; set; }

    public required string Operation { get; set; }

    public required string HttpMethod { get; set; }

    public required string Payload { get; set; }

    public OutboxMessageStatus Status { get; set; }

    public int Attempts { get; set; }

    public string? LastError { get; set; }

    public DateTime? SentAtUtc { get; set; }
}
