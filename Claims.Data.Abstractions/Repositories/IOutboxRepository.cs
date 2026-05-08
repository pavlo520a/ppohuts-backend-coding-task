using Claims.Domain.Models;

namespace Claims.Data.Abstractions.Repositories;

public interface IOutboxRepository
{
    void Add<TAuditOutbox>(TAuditOutbox auditOutbox)
        where TAuditOutbox : BaseAuditOutbox;

    Task MarkProcessingAsync(string id, CancellationToken cancellationToken);

    Task MarkSentAsync(string id, CancellationToken cancellationToken);

    Task RegisterAttemptAsync(string id, string error, CancellationToken cancellationToken);

    Task MarkFailedAsync(string id, CancellationToken cancellationToken);
}
