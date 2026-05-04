using Claims.Application.Commands;
using Claims.Data.Abstractions;
using Claims.Domain;

namespace Claims.Application.UseCases;

public sealed class CreateClaimUseCase(
    IClaimRepository claimRepository,
    IAuditTrailRepository auditTrailRepository) : ICreateClaimUseCase
{
    public async Task<Claim> ExecuteAsync(CreateClaimCommand command, CancellationToken cancellationToken = default)
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
        await auditTrailRepository.WriteClaimAuditAsync(claim.Id, "POST", cancellationToken);
        return claim;
    }
}
