using Claims.Application.Commands;
using Claims.Domain;

namespace Claims.Application.UseCases;

public interface IGetCoversUseCase
{
    Task<IReadOnlyList<Cover>> ExecuteAsync(GetCoversCommand command, CancellationToken cancellationToken = default);
}
