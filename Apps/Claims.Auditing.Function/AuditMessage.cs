namespace Claims.Auditing.Function;

public sealed class AuditMessage
{
    public required string EntityType { get; init; }

    public required string EntityId { get; init; }

    public required string HttpMethod { get; init; }

    public required DateTime OccurredAtUtc { get; init; }
}
