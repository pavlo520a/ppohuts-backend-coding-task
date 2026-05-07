using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions.Queries;
using Claims.Domain.Exceptions;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Covers;

public sealed class GetCoverByIdUseCase(ICoverQuery coverQuery) : IUseCase<GetCoverByIdCommand, Cover>
{
    public async Task<Cover> ExecuteAsync(GetCoverByIdCommand command, CancellationToken cancellationToken) =>
        await coverQuery.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new CoverNotFoundException(command.Id);
}
