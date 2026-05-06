using Claims.Data.Abstractions.Queries;
using Claims.Data.Mapping;
using Claims.Domain.Enums;
using Claims.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Queries;

public sealed class OutboxQuery(ClaimsMongoDbContext context) : IOutboxQuery
{
    public async Task<IReadOnlyList<OutboxMessage>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken)
    {
        var entities = await context.OutboxMessages
            .Where(x => x.Status == OutboxMessageStatus.Pending)
            .OrderBy(x => x.OccurredAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        return [.. entities.Select(entity => entity.ToDomain())];
    }
}
