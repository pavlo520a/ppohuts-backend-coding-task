using Claims.Domain;

namespace Claims.Data.Abstractions;

public interface ICoverRepository
{
    Task<IReadOnlyList<Cover>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task AddAsync(Cover cover, CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
