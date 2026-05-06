using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions.Repositories;

namespace Claims.Application.UseCases.Claims;

public sealed class DeleteClaimUseCase(
    IClaimRepository claimRepository) : IUseCase<DeleteClaimCommand>
{
    public async Task ExecuteAsync(DeleteClaimCommand command, CancellationToken cancellationToken)
    {
        await claimRepository.DeleteAsync(command.Id, command.HttpMethod, cancellationToken);
    }
}
