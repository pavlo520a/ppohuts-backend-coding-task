using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Repositories;
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

        unitOfWork.OutboxRepository.Add(auditOutbox);
        unitOfWork.CoversRepository.Delete(command.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
