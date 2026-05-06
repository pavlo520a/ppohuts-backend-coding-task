using Claims.Domain.Models;

namespace Claims.Data.Abstractions.Repositories;

public interface ICoverRepository
{
    Task<IReadOnlyList<Cover>> GetAllAsync(CancellationToken cancellationToken);

    Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task AddAsync(Cover cover, string httpMethod, CancellationToken cancellationToken);

    Task DeleteAsync(string id, string httpMethod, CancellationToken cancellationToken);
}
