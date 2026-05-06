using Claims.Domain.Models;

namespace Claims.Data.Abstractions.Queries;

public interface ICoverQuery
{
    Task<IReadOnlyList<Cover>> GetAllAsync(CancellationToken cancellationToken);

    Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken);
}
