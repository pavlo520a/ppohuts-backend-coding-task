using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions;

namespace Claims.Application.UseCases.Covers;

public sealed class DeleteCoverUseCase(
    ICoverRepository coverRepository,
    IAuditTrailRepository auditTrailRepository) : IUseCase<DeleteCoverCommand>
{
    public async Task ExecuteAsync(DeleteCoverCommand command, CancellationToken cancellationToken)
    {
        await auditTrailRepository.WriteCoverAuditAsync(command.Id, "DELETE", cancellationToken);
        await coverRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
