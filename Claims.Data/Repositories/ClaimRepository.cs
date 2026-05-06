using Claims.Data.Abstractions.Repositories;
using Claims.Data.Extensions;
using Claims.Data.Mapping;
using Claims.Domain.Constants;
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

        return entity?.ToDomain();
    }

    public async Task AddAsync(Claim claim, string httpMethod, CancellationToken cancellationToken)
    {
        var document = claim.ToDocument();

        context.Claims.Add(document);
        context.OutboxMessages.Add(document.ToOutboxMessage(AuditAggregateTypes.Claim, httpMethod));

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, string httpMethod, CancellationToken cancellationToken)
    {
        var entity = await context.Claims
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is not null)
        {
            context.Claims.Remove(entity);
            context.OutboxMessages.Add(entity.ToOutboxMessage(AuditAggregateTypes.Claim, httpMethod));

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
