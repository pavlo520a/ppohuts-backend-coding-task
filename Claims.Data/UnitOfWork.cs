using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Repositories;
using Claims.Data.Repositories;

namespace Claims.Data;

public sealed class UnitOfWork(ClaimsMongoDbContext context) : IUnitOfWork
{
    public IClaimRepository ClaimsRepository { get; } = new ClaimRepository(context);

    public ICoverRepository CoversRepository { get; } = new CoverRepository(context);

    public IOutboxRepository OutboxRepository { get; } = new OutboxRepository(context);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
