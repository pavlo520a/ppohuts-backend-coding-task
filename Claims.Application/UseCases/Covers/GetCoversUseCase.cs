using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions.Queries;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Covers;

public sealed class GetCoversUseCase(ICoverQuery coverQuery) : IUseCase<GetCoversCommand, IReadOnlyList<Cover>>
{
    public Task<IReadOnlyList<Cover>> ExecuteAsync(GetCoversCommand command, CancellationToken cancellationToken) =>
        coverQuery.GetAllAsync(cancellationToken);
}
