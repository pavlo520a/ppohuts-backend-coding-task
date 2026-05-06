using Claims.Data.Abstractions.Queries;
using Claims.Data.Mapping;
using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Queries;

public sealed class CoverQuery(ClaimsMongoDbContext context) : ICoverQuery
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
}
