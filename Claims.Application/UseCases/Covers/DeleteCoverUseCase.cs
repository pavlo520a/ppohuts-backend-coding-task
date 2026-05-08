using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions;
using Claims.Domain.Models;

namespace Claims.Application.UseCases.Covers;

public sealed class DeleteCoverUseCase(
    IUnitOfWork unitOfWork) : IUseCase<DeleteCoverCommand>
{
    public async Task ExecuteAsync(DeleteCoverCommand command, CancellationToken cancellationToken)
    {
        var auditOutbox = new AuditOutbox
        {
            EntityType = nameof(Cover),
            EntityId = command.Id,
            HttpMethod = command.HttpMethod,
            OccurredAtUtc = DateTime.UtcNow
        };

        unitOfWork.CoversRepository.Delete(command.Id);
        unitOfWork.OutboxRepository.Add(auditOutbox);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
