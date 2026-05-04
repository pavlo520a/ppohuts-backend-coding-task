using Claims.Application.Commands;

namespace Claims.Application.UseCases;

public interface IDeleteCoverUseCase
{
    Task ExecuteAsync(DeleteCoverCommand command, CancellationToken cancellationToken);
}
