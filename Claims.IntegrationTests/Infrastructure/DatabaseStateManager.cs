using Claims.Data;
using Claims.Data.Documents;
using Microsoft.EntityFrameworkCore;

namespace Claims.IntegrationTests.Infrastructure;

public sealed class DatabaseStateManager(
    ClaimsMongoDbContext claimsContext)
{
    public async Task ResetAsync()
    {
        var claims = await claimsContext.Set<ClaimDocument>().ToListAsync();
        var covers = await claimsContext.Set<CoverDocument>().ToListAsync();
        var outboxMessages = await claimsContext.Set<OutboxMessageDocument>().ToListAsync();

        claimsContext.RemoveRange(claims);
        claimsContext.RemoveRange(covers);
        claimsContext.RemoveRange(outboxMessages);

        await claimsContext.SaveChangesAsync();
    }
}
