using Claims.Data.Abstractions;
using Claims.Data.Persistence;
using Claims.Domain;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data;

public sealed class CoverRepository(ClaimsMongoDbContext context) : ICoverRepository
{
    public async Task<IReadOnlyList<Cover>> GetAllAsync(CancellationToken cancellationToken)
    {
        var items = await context.Covers.ToListAsync(cancellationToken);
        return [.. items.Select(ToDomain)];
    }

    public async Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await context.Covers
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        return entity is null
            ? null
            : ToDomain(entity);
    }

    public async Task AddAsync(Cover cover, CancellationToken cancellationToken)
    {
        context.Covers.Add(ToDocument(cover));
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

    private static Cover ToDomain(CoverDocument d) =>
        new()
        {
            Id = d.Id,
            StartDate = d.StartDate,
            EndDate = d.EndDate,
            Type = d.Type,
            Premium = d.Premium
        };

    private static CoverDocument ToDocument(Cover c) =>
        new()
        {
            Id = c.Id,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            Type = c.Type,
            Premium = c.Premium
        };
}
