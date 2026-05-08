using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions.Queries;
using Claims.Domain.Exceptions;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Claims;

public sealed class GetClaimByIdUseCase(IClaimQuery claimQuery) : IUseCase<GetClaimByIdCommand, Claim>
{
    public async Task<Claim> ExecuteAsync(GetClaimByIdCommand command, CancellationToken cancellationToken) =>
        await claimQuery.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new ClaimNotFoundException(command.Id);
}
