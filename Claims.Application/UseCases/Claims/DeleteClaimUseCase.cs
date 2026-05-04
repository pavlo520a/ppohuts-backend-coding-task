using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions;

namespace Claims.Application.UseCases.Claims;

public sealed class DeleteClaimUseCase(
    IClaimRepository claimRepository,
    IAuditTrailRepository auditTrailRepository) : IUseCase<DeleteClaimCommand>
{
    public async Task ExecuteAsync(DeleteClaimCommand command, CancellationToken cancellationToken)
    {
        await auditTrailRepository.WriteClaimAuditAsync(command.Id, "DELETE", cancellationToken);
        await claimRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
