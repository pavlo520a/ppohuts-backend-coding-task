namespace Claims.Domain.Models;

public sealed class AuditOutbox : BaseAuditOutbox
{
    public required string HttpMethod { get; set; }

    public required DateTime OccurredAtUtc { get; set; }
}
