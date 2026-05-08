using Claims.Web.ApiModels.Claims;
using Claims.Application.Commands.Claims;
using Claims.Domain.Models;

namespace Claims.Web.Mapping;

public static class ClaimApiMapping
{
    public static CreateClaimCommand ToCommand(this CreateClaimRequest request, string httpMethod) =>
        new()
        {
            CoverId = request.CoverId,
            Created = request.Created,
            Name = request.Name,
            Type = request.Type,
            DamageCost = request.DamageCost,
            HttpMethod = httpMethod
        };

    public static ClaimResponse ToResponse(this Claim claim) =>
        new()
        {
            Id = claim.Id,
            CoverId = claim.CoverId,
            Created = claim.Created,
            Name = claim.Name,
            Type = claim.Type,
            DamageCost = claim.DamageCost
        };

    public static IReadOnlyList<ClaimResponse> ToResponse(this IEnumerable<Claim> claims) =>
        [.. claims.Select(c => c.ToResponse())];
}
