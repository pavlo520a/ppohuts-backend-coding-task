using Claims.Data.Abstractions.Outbox;
using Claims.Data.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data.Repositories;

public sealed class OutboxRepository(ClaimsMongoDbContext context) : IOutboxRepository
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

    public async Task MarkProcessingAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await context.OutboxMessages.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return;
        }

        entity.Status = OutboxMessageStatus.Processing;
        entity.LastError = null;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkSentAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await context.OutboxMessages.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        if (entity is null)
        {
            return;
        }

        entity.Status = OutboxMessageStatus.Sent;
        entity.SentAtUtc = DateTime.UtcNow;
        entity.LastError = null;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(string id, string error, CancellationToken cancellationToken)
    {
        var entity = await context.OutboxMessages.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        if (entity is null)
        {
            return;
        }

        entity.Status = OutboxMessageStatus.Pending;
        entity.Attempts += 1;
        entity.LastError = error;

        await context.SaveChangesAsync(cancellationToken);
    }
}
