using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions.Repositories;
using Claims.Data.Auditing.Abstractions.Repositories;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Claims;

public sealed class CreateClaimUseCase(
    IClaimRepository claimRepository,
    IClaimAuditTrailRepository claimAuditTrailRepository) : IUseCase<CreateClaimCommand, Claim>
{
    public async Task<Claim> ExecuteAsync(CreateClaimCommand command, CancellationToken cancellationToken)
    {
        var claim = new Claim
        {
            Id = Guid.NewGuid().ToString(),
            CoverId = command.CoverId,
            Created = command.Created,
            Name = command.Name,
            Type = command.Type,
            DamageCost = command.DamageCost
        };

        await claimRepository.AddAsync(claim, cancellationToken);
        await claimAuditTrailRepository.WriteAsync(claim.Id, "POST", cancellationToken);
        return claim;
    }
}
