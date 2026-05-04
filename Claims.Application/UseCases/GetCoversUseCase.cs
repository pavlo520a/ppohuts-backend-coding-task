using Claims.Application.Commands;
using Claims.Data.Abstractions;
using Claims.Domain;

namespace Claims.Application.UseCases;

public sealed class GetCoversUseCase(ICoverRepository coverRepository) : IGetCoversUseCase
{
    public Task<IReadOnlyList<Cover>> ExecuteAsync(GetCoversCommand command, CancellationToken cancellationToken = default) =>
        coverRepository.GetAllAsync(cancellationToken);
}
