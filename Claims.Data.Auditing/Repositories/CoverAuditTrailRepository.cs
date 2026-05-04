using Claims.Data.Auditing.Abstractions.Repositories;
using Claims.Data.Auditing.Entities;

namespace Claims.Data.Auditing.Repositories;
public sealed class CoverAuditTrailRepository(AuditContext auditContext) : ICoverAuditTrailRepository
{
    public async Task WriteAsync(string coverId, string httpMethod, CancellationToken cancellationToken)
    {
        auditContext.CoverAudits.Add(new CoverAudit
        {
            Created = DateTime.UtcNow,
            HttpRequestType = httpMethod,
            CoverId = coverId
        });

        await auditContext.SaveChangesAsync(cancellationToken);
    }
}
