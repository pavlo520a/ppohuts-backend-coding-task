using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions.Repositories;

namespace Claims.Application.UseCases.Covers;

public sealed class DeleteCoverUseCase(
    ICoverRepository coverRepository) : IUseCase<DeleteCoverCommand>
{
    public async Task ExecuteAsync(DeleteCoverCommand command, CancellationToken cancellationToken)
    {
        await coverRepository.DeleteAsync(command.Id, command.HttpMethod, cancellationToken);
    }
}
