using Claims.Data.Abstractions.Repositories;
using Claims.Data.Extensions;
using Claims.Data.Mapping;
using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Repositories;

public sealed class CoverRepository(ClaimsMongoDbContext context) : ICoverRepository
{
    public async Task<IReadOnlyList<Cover>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = await context.Covers.ToListAsync(cancellationToken);
        return [.. items.Select(item => item.ToDomain())];
    }

    public async Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await context.Covers
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        return entity?.ToDomain();
    }

    public async Task AddAsync(Cover cover, string httpMethod, CancellationToken cancellationToken)
    {
        var document = cover.ToDocument();

        context.Covers.Add(document);
        context.OutboxMessages.Add(document.ToOutboxMessage(httpMethod));

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, string httpMethod, CancellationToken cancellationToken)
    {
        var entity = await context.Covers
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is not null)
        {
            context.Covers.Remove(entity);
            context.OutboxMessages.Add(entity.ToOutboxMessage(httpMethod));

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
