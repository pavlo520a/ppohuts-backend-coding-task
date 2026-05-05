using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Exceptions;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Covers;

public sealed class GetCoverByIdUseCase(ICoverRepository coverRepository) : IUseCase<GetCoverByIdCommand, Cover>
{
    public async Task<Cover> ExecuteAsync(GetCoverByIdCommand command, CancellationToken cancellationToken) =>
        await coverRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new CoverNotFoundException(command.Id);
}
