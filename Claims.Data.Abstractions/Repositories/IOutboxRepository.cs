using Claims.Domain.Models;

namespace Claims.Data.Abstractions.Repositories;

public interface IOutboxRepository
{
    Task<IReadOnlyList<OutboxMessage>> GetPendingBatchAsync(int batchSize, CancellationToken cancellationToken);

    Task MarkProcessingAsync(string id, CancellationToken cancellationToken);

    Task MarkSentAsync(string id, CancellationToken cancellationToken);

    Task MarkFailedAsync(string id, string error, CancellationToken cancellationToken);
}
