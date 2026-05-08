using Claims.Data.Auditing.Abstractions.Repositories;
using Claims.Data.Auditing.Entities;

using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Auditing.Repositories;

public sealed class ClaimAuditTrailRepository(AuditContext context) : IClaimAuditTrailRepository
{
    public Task<bool> AnyAsync(string claimId, string httpMethod, CancellationToken cancellationToken)
    {
        return context.ClaimAudits.AnyAsync(
            x => x.ClaimId == claimId && x.HttpRequestType == httpMethod,
            cancellationToken);
    }

    public async Task AddAsync(string claimId, string httpMethod, CancellationToken cancellationToken)
    {
        context.ClaimAudits.Add(new ClaimAudit
        {
            Created = DateTime.UtcNow,
            HttpRequestType = httpMethod,
            ClaimId = claimId
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
