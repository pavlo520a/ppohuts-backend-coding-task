using Claims.Application.Commands;
using Claims.Data.Abstractions;
using Claims.Domain;

namespace Claims.Application.UseCases;

public sealed class GetCoverByIdUseCase(ICoverRepository coverRepository) : IGetCoverByIdUseCase
{
    public Task<Cover?> ExecuteAsync(GetCoverByIdCommand command, CancellationToken cancellationToken) =>
        coverRepository.GetByIdAsync(command.Id, cancellationToken);
}
