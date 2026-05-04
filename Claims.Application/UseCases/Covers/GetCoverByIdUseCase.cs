using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Covers;

public sealed class GetCoverByIdUseCase(ICoverRepository coverRepository) : IUseCase<GetCoverByIdCommand, Cover?>
{
    public Task<Cover?> ExecuteAsync(GetCoverByIdCommand command, CancellationToken cancellationToken) =>
        coverRepository.GetByIdAsync(command.Id, cancellationToken);
}
