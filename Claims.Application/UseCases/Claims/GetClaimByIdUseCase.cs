using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Exceptions;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Claims;

public sealed class GetClaimByIdUseCase(IClaimRepository claimRepository) : IUseCase<GetClaimByIdCommand, Claim>
{
    public async Task<Claim> ExecuteAsync(GetClaimByIdCommand command, CancellationToken cancellationToken) =>
        await claimRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new ClaimNotFoundException(command.Id);
}
