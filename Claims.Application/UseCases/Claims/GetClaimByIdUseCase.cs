using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Claims;

public sealed class GetClaimByIdUseCase(IClaimRepository claimRepository) : IUseCase<GetClaimByIdCommand, Claim?>
{
    public Task<Claim?> ExecuteAsync(GetClaimByIdCommand command, CancellationToken cancellationToken) =>
        claimRepository.GetByIdAsync(command.Id, cancellationToken);
}
