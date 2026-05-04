using Claims.Domain;

namespace Claims.Data.Abstractions;

public interface IClaimRepository
{
    Task<IReadOnlyList<Claim>> GetAllAsync(CancellationToken cancellationToken);

    Task<Claim?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task AddAsync(Claim claim, CancellationToken cancellationToken);

    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
