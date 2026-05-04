using Claims.Auditing;
using Claims.Data.Abstractions;

namespace Claims.Data.Auditing;

public sealed class SqlAuditTrailRepository(AuditContext auditContext) : IAuditTrailRepository
{
    public async Task WriteClaimAuditAsync(string claimId, string httpMethod, CancellationToken cancellationToken)
    {
        var claimAudit = new ClaimAudit
        {
            Created = DateTime.UtcNow,
            HttpRequestType = httpMethod,
            ClaimId = claimId
        };

        auditContext.ClaimAudits.Add(claimAudit);
        await auditContext.SaveChangesAsync(cancellationToken);
    }

    public async Task WriteCoverAuditAsync(string coverId, string httpMethod, CancellationToken cancellationToken)
    {
        var coverAudit = new CoverAudit
        {
            Created = DateTime.UtcNow,
            HttpRequestType = httpMethod,
            CoverId = coverId
        };

        auditContext.CoverAudits.Add(coverAudit);
        await auditContext.SaveChangesAsync(cancellationToken);
    }
}
