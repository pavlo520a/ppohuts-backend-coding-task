using Claims.Domain.Models;

namespace Claims.Data.Abstractions.Repositories;

public interface IOutboxRepository
{
    void Add(BaseAuditOutbox auditOutbox);

    Task MarkProcessingAsync(string id, CancellationToken cancellationToken);

    Task MarkSentAsync(string id, CancellationToken cancellationToken);

    Task RegisterAttemptAsync(string id, string error, CancellationToken cancellationToken);

    Task MarkFailedAsync(string id, CancellationToken cancellationToken);
}
