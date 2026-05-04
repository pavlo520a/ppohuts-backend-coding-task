using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Claims;

public sealed class GetClaimsUseCase(IClaimRepository claimRepository) : IUseCase<GetClaimsCommand, IReadOnlyList<Claim>>
{
    public Task<IReadOnlyList<Claim>> ExecuteAsync(GetClaimsCommand command, CancellationToken cancellationToken) =>
        claimRepository.GetAllAsync(cancellationToken);
}
