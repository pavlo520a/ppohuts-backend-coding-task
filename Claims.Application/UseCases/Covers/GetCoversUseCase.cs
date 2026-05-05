using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Covers;

public sealed class GetCoversUseCase(ICoverRepository coverRepository) : IUseCase<GetCoversCommand, IReadOnlyList<Cover>>
{
    public Task<IReadOnlyList<Cover>> ExecuteAsync(GetCoversCommand command, CancellationToken cancellationToken) =>
        coverRepository.GetAllAsync(cancellationToken);
}
