using Claims.ApiModels.Claims;
using Claims.ApiModels.Covers;
using Claims.ApiModels.Formulas;
using Claims.Domain.Enums;

namespace Claims.IntegrationTests.Infrastructure;

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

    public static CreateClaimRequest CreateValidClaimRequest(
        string coverId,
        DateTime? created = null,
        ClaimType type = ClaimType.Collision)
    {
        return new CreateClaimRequest
        {
            CoverId = coverId,
            Created = created ?? DateTime.UtcNow.Date.AddDays(1),
            Name = "Test claim",
            Type = type,
            DamageCost = 1000m
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
