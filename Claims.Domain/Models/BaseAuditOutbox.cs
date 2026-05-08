namespace Claims.Domain.Models;

public abstract class BaseAuditOutbox
{
    public required string EntityType { get; init; }

    public required string EntityId { get; init; }
}
