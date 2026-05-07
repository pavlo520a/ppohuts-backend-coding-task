using Claims.Data;
using Claims.Data.Auditing;
using Claims.Data.Documents;
using Claims.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.IntegrationTests.Infrastructure;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly ClaimsWebApplicationFactory _factory = new();

    protected HttpClient Client { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        Client = _factory.CreateClient();
        await _factory.ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        _factory.Dispose();

        return ValueTask.CompletedTask;
    }

    protected async Task SeedCoverAsync(CoverDocument cover)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ClaimsMongoDbContext>();
        context.Covers.Add(cover);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    protected async Task SeedClaimAsync(ClaimDocument claim)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ClaimsMongoDbContext>();
        context.Claims.Add(claim);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    protected async Task<OutboxMessageDocument[]> GetOutboxMessagesAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ClaimsMongoDbContext>();

        return await context.OutboxMessages
            .OrderBy(x => x.OccurredAtUtc)
            .ToArrayAsync(TestContext.Current.CancellationToken);
    }

    protected async Task<bool> ClaimExistsAsync(string claimId)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ClaimsMongoDbContext>();
        return await context.Claims.AnyAsync(x => x.Id == claimId, TestContext.Current.CancellationToken);
    }

    protected async Task<bool> CoverExistsAsync(string coverId)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ClaimsMongoDbContext>();
        return await context.Covers.AnyAsync(x => x.Id == coverId, TestContext.Current.CancellationToken);
    }

    protected static CoverDocument CreateCoverDocument(
        string id,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CoverType type = CoverType.Yacht,
        decimal premium = 1200m)
    {
        var start = DateOnly.FromDateTime((startDate ?? DateTime.UtcNow.Date.AddDays(1)).Date);
        var end = DateOnly.FromDateTime((endDate ?? DateTime.UtcNow.Date.AddDays(10)).Date);

        return new CoverDocument
        {
            Id = id,
            StartDate = start,
            EndDate = end,
            Type = type,
            Premium = premium
        };
    }

    protected static ClaimDocument CreateClaimDocument(
        string id,
        string coverId,
        DateTime? created = null,
        ClaimType type = ClaimType.Fire,
        decimal damageCost = 500m,
        string? name = null)
    {
        return new ClaimDocument
        {
            Id = id,
            CoverId = coverId,
            Created = created ?? DateTime.UtcNow.Date.AddDays(1),
            Name = name ?? "Seeded claim",
            Type = type,
            DamageCost = damageCost
        };
    }
}
