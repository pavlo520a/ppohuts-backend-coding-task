using Claims.Data.Auditing.Abstractions.Repositories;
using Claims.Data.Auditing.Entities;

using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Auditing.Repositories;
public sealed class CoverAuditTrailRepository(AuditContext context) : ICoverAuditTrailRepository
{
    public Task<bool> AnyAsync(string coverId, string httpMethod, CancellationToken cancellationToken)
    {
        return context.CoverAudits.AnyAsync(
            x => x.CoverId == coverId && x.HttpRequestType == httpMethod,
            cancellationToken);
    }

    public async Task WriteAsync(string coverId, string httpMethod, CancellationToken cancellationToken)
    {
        context.CoverAudits.Add(new CoverAudit
        {
            Created = DateTime.UtcNow,
            HttpRequestType = httpMethod,
            CoverId = coverId
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
