using Claims.ApiModels.Claims;
using Claims.ApiModels.Covers;
using Claims.ApiModels.Formulas;
using Claims.Domain.Enums;

namespace Claims.IntegrationTests.Infrastructure.Factories;

public static class TestDataFactory
{
    public static CreateCoverRequest CreateValidCoverRequest(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CoverType type = CoverType.Yacht)
    {
        var start = startDate ?? DateTime.UtcNow.Date.AddDays(1);
        var end = endDate ?? start.AddDays(10);

        return new CreateCoverRequest
        {
            StartDate = start,
            EndDate = end,
            Type = type
        };
    }

    public static CreateClaimRequest CreateClaimRequest(
        string coverId,
        DateTime? created = null,
        ClaimType type = ClaimType.Collision,
        decimal damageCost = 1000m,
        string? name = null)
    {
        return new CreateClaimRequest
        {
            CoverId = coverId,
            Created = created ?? DateTime.UtcNow.Date.AddDays(1),
            Name = name ?? "Test claim",
            Type = type,
            DamageCost = damageCost
        };
    }

    public static ComputePremiumRequest CreateValidPremiumRequest(
        DateTime? startDate = null,
        DateTime? endDate = null,
        CoverType type = CoverType.Yacht)
    {
        var start = startDate ?? DateTime.UtcNow.Date.AddDays(1);
        var end = endDate ?? start.AddDays(30);

        return new ComputePremiumRequest
        {
            StartDate = start,
            EndDate = end,
            CoverType = type
        };
    }
}
