using Claims.Application.Commands;
using Claims.Data.Abstractions;

namespace Claims.Application.UseCases;

public sealed class DeleteClaimUseCase(
    IClaimRepository claimRepository,
    IAuditTrailRepository auditTrailRepository) : IDeleteClaimUseCase
{
    public async Task ExecuteAsync(DeleteClaimCommand command, CancellationToken cancellationToken = default)
    {
        await auditTrailRepository.WriteClaimAuditAsync(command.Id, "DELETE", cancellationToken);
        await claimRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
