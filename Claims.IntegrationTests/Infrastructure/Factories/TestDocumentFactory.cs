using Claims.Data.Documents;
using Claims.Domain.Enums;

namespace Claims.IntegrationTests.Infrastructure.Factories;

public static class TestDocumentFactory
{
    public static CoverDocument CreateCoverDocument(
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

    public static ClaimDocument CreateClaimDocument(
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
