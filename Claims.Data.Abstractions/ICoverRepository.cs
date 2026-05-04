using Claims.Domain.Models;

namespace Claims.Data.Abstractions;

public interface ICoverRepository
{
    Task<IReadOnlyList<Cover>> GetAllAsync(CancellationToken cancellationToken);

    Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task AddAsync(Cover cover, CancellationToken cancellationToken);

    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
