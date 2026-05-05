namespace Claims.Data.Auditing.Abstractions.Repositories;

public interface IClaimAuditTrailRepository
{
    Task WriteAsync(string claimId, string httpMethod, CancellationToken cancellationToken);
}
