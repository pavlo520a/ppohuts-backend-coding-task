using Claims.Application.Commands;
using Claims.Domain;

namespace Claims.Application.UseCases;

public interface IGetClaimByIdUseCase
{
    Task<Claim?> ExecuteAsync(GetClaimByIdCommand command, CancellationToken cancellationToken);
}
