using Claims.Data.Abstractions.Repositories;
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

    public async Task AddAsync(Cover cover, CancellationToken cancellationToken)
    {
        context.Covers.Add(cover.ToDocument());
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await context.Covers
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is not null)
        {
            context.Covers.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
