using Claims.ApiModels;
using Claims.Application.Commands.Claims;
using Claims.Domain.Models;

namespace Claims.Mapping;

public static class ClaimApiMapping
{
    public static CreateClaimCommand ToCommand(this CreateClaimRequest request) =>
        new(request.CoverId, request.Created, request.Name, request.Type, request.DamageCost);

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
}
