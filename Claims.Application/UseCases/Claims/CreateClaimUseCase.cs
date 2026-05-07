using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions;
using Claims.Domain.Models;
using FluentValidation;

namespace Claims.Application.UseCases.Claims;

public sealed class CreateClaimUseCase(
    IUnitOfWork unitOfWork,
    IValidator<CreateClaimCommand> validator) : IUseCase<CreateClaimCommand, Claim>
{
    public async Task<Claim> ExecuteAsync(CreateClaimCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var claim = new Claim
        {
            Id = Guid.NewGuid().ToString(),
            CoverId = command.CoverId,
            Created = command.Created,
            Name = command.Name,
            Type = command.Type,
            DamageCost = command.DamageCost
        };

        var auditOutbox = new AuditOutbox
        {
            EntityType = nameof(Claim),
            EntityId = claim.Id,
            HttpMethod = command.HttpMethod,
            OccurredAtUtc = DateTime.UtcNow
        };

        unitOfWork.ClaimsRepository.Add(claim);
        unitOfWork.OutboxRepository.Add(auditOutbox);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return claim;
    }
}
