using Claims.Application.Commands;
using Claims.Data.Abstractions;

namespace Claims.Application.UseCases;

public sealed class DeleteCoverUseCase(
    ICoverRepository coverRepository,
    IAuditTrailRepository auditTrailRepository) : IDeleteCoverUseCase
{
    public async Task ExecuteAsync(DeleteCoverCommand command, CancellationToken cancellationToken = default)
    {
        await auditTrailRepository.WriteCoverAuditAsync(command.Id, "DELETE", cancellationToken);
        await coverRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
