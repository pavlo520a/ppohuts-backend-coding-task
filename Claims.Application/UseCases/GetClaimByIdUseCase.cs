using Claims.Application.Commands;
using Claims.Data.Abstractions;
using Claims.Domain;

namespace Claims.Application.UseCases;

public sealed class GetClaimByIdUseCase(IClaimRepository claimRepository) : IGetClaimByIdUseCase
{
    public Task<Claim?> ExecuteAsync(GetClaimByIdCommand command, CancellationToken cancellationToken) =>
        claimRepository.GetByIdAsync(command.Id, cancellationToken);
}
