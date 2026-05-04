using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Data.Abstractions.Repositories;
using Claims.Data.Auditing.Abstractions.Repositories;

namespace Claims.Application.UseCases.Claims;

public sealed class DeleteClaimUseCase(
    IClaimRepository claimRepository,
    IClaimAuditTrailRepository claimAuditTrailRepository) : IUseCase<DeleteClaimCommand>
{
    public async Task ExecuteAsync(DeleteClaimCommand command, CancellationToken cancellationToken)
    {
        await claimAuditTrailRepository.WriteAsync(command.Id, "DELETE", cancellationToken);
        await claimRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
