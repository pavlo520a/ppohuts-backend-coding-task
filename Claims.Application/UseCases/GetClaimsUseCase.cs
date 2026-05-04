using Claims.Application.Commands;
using Claims.Data.Abstractions;
using Claims.Domain;

namespace Claims.Application.UseCases;

public sealed class GetClaimsUseCase(IClaimRepository claimRepository) : IGetClaimsUseCase
{
    public Task<IReadOnlyList<Claim>> ExecuteAsync(GetClaimsCommand command, CancellationToken cancellationToken = default) =>
        claimRepository.GetAllAsync(cancellationToken);
}
