using Claims.Application.Abstractions;
using Claims.Application.Commands.Covers;
using Claims.Data.Abstractions.Repositories;
using Claims.Data.Auditing.Abstractions.Repositories;

namespace Claims.Application.UseCases.Covers;

public sealed class DeleteCoverUseCase(
    ICoverRepository coverRepository,
    ICoverAuditTrailRepository coverAuditTrailRepository) : IUseCase<DeleteCoverCommand>
{
    public async Task ExecuteAsync(DeleteCoverCommand command, CancellationToken cancellationToken)
    {
        await coverAuditTrailRepository.WriteAsync(command.Id, command.HttpMethod, cancellationToken);
        await coverRepository.DeleteAsync(command.Id, cancellationToken);
    }
}
