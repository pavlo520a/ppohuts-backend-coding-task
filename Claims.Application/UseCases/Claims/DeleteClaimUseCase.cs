using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Claims;

public sealed class DeleteClaimUseCase(
    IUnitOfWork unitOfWork) : IUseCase<DeleteClaimCommand>
{
    public async Task ExecuteAsync(DeleteClaimCommand command, CancellationToken cancellationToken)
    {
        var auditOutbox = new AuditOutbox
        {
            EntityType = nameof(Claim),
            EntityId = command.Id,
            HttpMethod = command.HttpMethod,
            OccurredAtUtc = DateTime.UtcNow
        };

        unitOfWork.ClaimsRepository.Delete(command.Id);
        unitOfWork.OutboxRepository.Add(auditOutbox);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
