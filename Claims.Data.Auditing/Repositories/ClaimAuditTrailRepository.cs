using Claims.Data.Auditing.Abstractions.Repositories;
using Claims.Data.Auditing.Entities;

namespace Claims.Data.Auditing.Repositories;

public sealed class ClaimAuditTrailRepository(AuditContext auditContext) : IClaimAuditTrailRepository
{
    public async Task WriteAsync(string claimId, string httpMethod, CancellationToken cancellationToken)
    {
        auditContext.ClaimAudits.Add(new ClaimAudit
        {
            Created = DateTime.UtcNow,
            HttpRequestType = httpMethod,
            ClaimId = claimId
        });

        await auditContext.SaveChangesAsync(cancellationToken);
    }
}
