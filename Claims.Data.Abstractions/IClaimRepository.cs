using Claims.Domain;

namespace Claims.Data.Abstractions;

public interface IClaimRepository
{
    Task<IReadOnlyList<Claim>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Claim?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task AddAsync(Claim claim, CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
