using Claims.Data.Abstractions.Repositories;

namespace Claims.Data.Abstractions;

public interface IUnitOfWork
{
    IClaimRepository ClaimsRepository { get; }

    ICoverRepository CoversRepository { get; }

    IOutboxRepository OutboxRepository { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
