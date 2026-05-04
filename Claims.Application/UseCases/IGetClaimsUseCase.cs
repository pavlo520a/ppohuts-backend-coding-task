using Claims.Application.Commands;
using Claims.Domain;

namespace Claims.Application.UseCases;

public interface IGetClaimsUseCase
{
    Task<IReadOnlyList<Claim>> ExecuteAsync(GetClaimsCommand command, CancellationToken cancellationToken);
}
