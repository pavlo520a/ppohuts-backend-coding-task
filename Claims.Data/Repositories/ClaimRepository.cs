using Claims.Data.Abstractions.Repositories;
using Claims.Data.Mapping;
using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Repositories;

public sealed class ClaimRepository(ClaimsMongoDbContext context) : IClaimRepository
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

        return entity is null
            ? null
            : entity.ToDomain();
    }

    public async Task AddAsync(Claim claim, CancellationToken cancellationToken)
    {
        context.Claims.Add(claim.ToDocument());
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await context.Claims
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is not null)
        {
            context.Claims.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
