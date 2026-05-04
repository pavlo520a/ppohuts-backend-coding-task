using Claims.Application.Commands;

namespace Claims.Application.UseCases;

public interface IDeleteClaimUseCase
{
    Task ExecuteAsync(DeleteClaimCommand command, CancellationToken cancellationToken = default);
}
