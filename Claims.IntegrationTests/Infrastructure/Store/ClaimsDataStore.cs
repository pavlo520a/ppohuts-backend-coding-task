using Claims.Data;
using Claims.Data.Documents;
using Microsoft.EntityFrameworkCore;

namespace Claims.IntegrationTests.Infrastructure.Store;

public sealed class ClaimsDataStore(
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

    public async Task SeedCoverAsync(CoverDocument cover)
    {
        claimsContext.Covers.Add(cover);
        await claimsContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public async Task SeedClaimAsync(ClaimDocument claim)
    {
        claimsContext.Claims.Add(claim);
        await claimsContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public Task<OutboxMessageDocument[]> GetOutboxMessagesAsync()
    {
        return claimsContext.OutboxMessages
            .OrderBy(x => x.OccurredAtUtc)
            .ToArrayAsync(TestContext.Current.CancellationToken);
    }

    public Task<bool> ClaimExistsAsync(string claimId)
    {
        return claimsContext.Claims.AnyAsync(x => x.Id == claimId, TestContext.Current.CancellationToken);
    }

    public Task<bool> CoverExistsAsync(string coverId)
    {
        return claimsContext.Covers.AnyAsync(x => x.Id == coverId, TestContext.Current.CancellationToken);
    }
}
