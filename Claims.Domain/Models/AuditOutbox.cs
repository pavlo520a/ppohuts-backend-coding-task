namespace Claims.Domain.Models;

public sealed class AuditOutbox
{
    public required string EntityType { get; init; }

    public required string EntityId { get; init; }

    public required string HttpMethod { get; init; }

    public required DateTime OccurredAtUtc { get; init; }
}
