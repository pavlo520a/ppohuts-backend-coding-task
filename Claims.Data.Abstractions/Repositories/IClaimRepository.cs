using Claims.Domain.Models;

namespace Claims.Data.Abstractions.Repositories;

public interface IClaimRepository
{
    Task<IReadOnlyList<Claim>> GetAllAsync(CancellationToken cancellationToken);

    Task<Claim?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task AddAsync(Claim claim, CancellationToken cancellationToken);

    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
