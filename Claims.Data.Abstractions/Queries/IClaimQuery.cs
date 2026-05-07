using Claims.Domain.Models;

namespace Claims.Data.Abstractions.Queries;

public interface IClaimQuery
{
    Task<IReadOnlyList<Claim>> GetAllAsync(CancellationToken cancellationToken);

    Task<Claim?> GetByIdAsync(string id, CancellationToken cancellationToken);
}
