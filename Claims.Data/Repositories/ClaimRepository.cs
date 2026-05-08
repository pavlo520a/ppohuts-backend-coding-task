using Claims.Data.Abstractions.Repositories;
using Claims.Data.Documents;
using Claims.Data.Mapping;
using Claims.Domain.Models;

namespace Claims.Data.Repositories;

public sealed class ClaimRepository(ClaimsMongoDbContext context) : IClaimRepository
{
    public void Add(Claim claim)
    {
        var document = claim.ToDocument();
        context.Claims.Add(document);
    }

    public void Delete(string id)
    {
        context.Claims.Remove(new ClaimDocument
        {
            Id = id,
            CoverId = null!,
            Name = null!
        });
    }
}
