namespace Claims.Data.Auditing.Abstractions.Repositories;

public interface IClaimAuditTrailRepository
{
    Task<bool> AnyAsync(string claimId, string httpMethod, CancellationToken cancellationToken);

    Task AddAsync(string claimId, string httpMethod, CancellationToken cancellationToken);
}
