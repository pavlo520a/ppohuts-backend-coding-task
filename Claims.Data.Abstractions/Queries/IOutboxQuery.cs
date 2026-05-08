using Claims.Domain.Models;

namespace Claims.Data.Abstractions.Queries;

public interface IOutboxQuery
{
    Task<IReadOnlyList<OutboxMessage>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken);
}
