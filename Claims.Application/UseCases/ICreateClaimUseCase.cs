using Claims.Application.Commands;
using Claims.Domain;

namespace Claims.Application.UseCases;

public interface ICreateClaimUseCase
{
    Task<Claim> ExecuteAsync(CreateClaimCommand command, CancellationToken cancellationToken);
}
