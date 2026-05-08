using Claims.Data.Documents;
using Claims.Domain.Models;

namespace Claims.Data.Mapping;

public static class ClaimMapping
{
    public static Claim ToDomain(this ClaimDocument document) =>
        new()
        {
            Id = document.Id,
            CoverId = document.CoverId,
            Created = document.Created,
            Name = document.Name,
            Type = document.Type,
            DamageCost = document.DamageCost
        };

    public static ClaimDocument ToDocument(this Claim claim) =>
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
