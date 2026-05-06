using Claims.Data.Abstractions.Queries;
using Claims.Data.Mapping;
using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Queries;

public sealed class ClaimQuery(ClaimsMongoDbContext context) : IClaimQuery
{
    public async Task<IReadOnlyList<Claim>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = await context.Claims.ToListAsync(cancellationToken);
        return [.. items.Select(item => item.ToDomain())];
    }

    public async Task<Claim?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await context.Claims
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        return entity?.ToDomain();
    }
}
