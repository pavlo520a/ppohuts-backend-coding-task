using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions.Queries;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Claims;

public sealed class GetClaimsUseCase(IClaimQuery claimQuery) : IUseCase<GetClaimsCommand, IReadOnlyList<Claim>>
{
    public Task<IReadOnlyList<Claim>> ExecuteAsync(GetClaimsCommand command, CancellationToken cancellationToken) =>
        claimQuery.GetAllAsync(cancellationToken);
}
