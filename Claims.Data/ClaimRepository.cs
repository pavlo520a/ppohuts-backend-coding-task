using Claims.Data.Abstractions;
using Claims.Data.Persistence;
using Claims.Domain;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data;

public sealed class ClaimRepository(ClaimsMongoDbContext context) : IClaimRepository
{
    public async Task<IReadOnlyList<Claim>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = await context.Claims.ToListAsync(cancellationToken);
        return items.Select(ToDomain).ToList();
    }

    public async Task<Claim?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await context.Claims
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task AddAsync(Claim claim, CancellationToken cancellationToken)
    {
        context.Claims.Add(ToDocument(claim));
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

    private static Claim ToDomain(ClaimDocument d) =>
        new()
        {
            Id = d.Id,
            CoverId = d.CoverId,
            Created = d.Created,
            Name = d.Name,
            Type = d.Type,
            DamageCost = d.DamageCost
        };

    private static ClaimDocument ToDocument(Claim c) =>
        new()
        {
            Id = c.Id,
            CoverId = c.CoverId,
            Created = c.Created,
            Name = c.Name,
            Type = c.Type,
            DamageCost = c.DamageCost
        };
}
