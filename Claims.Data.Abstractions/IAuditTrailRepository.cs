namespace Claims.Data.Abstractions;

public interface IAuditTrailRepository
{
    Task WriteClaimAuditAsync(string claimId, string httpMethod, CancellationToken cancellationToken = default);

    Task WriteCoverAuditAsync(string coverId, string httpMethod, CancellationToken cancellationToken = default);
}
