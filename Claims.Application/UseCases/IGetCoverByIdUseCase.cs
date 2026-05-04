using Claims.Application.Commands;
using Claims.Domain;

namespace Claims.Application.UseCases;

public interface IGetCoverByIdUseCase
{
    Task<Cover?> ExecuteAsync(GetCoverByIdCommand command, CancellationToken cancellationToken);
}
