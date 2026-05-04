using Claims.Application.Commands;
using Claims.Domain;

namespace Claims.Application.UseCases;

public interface ICreateCoverUseCase
{
    Task<Cover> ExecuteAsync(CreateCoverCommand command, CancellationToken cancellationToken = default);
}
