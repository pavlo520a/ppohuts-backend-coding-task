using Claims.Application.Commands;

namespace Claims.Application.UseCases;

public interface IComputePremiumUseCase
{
    Task<decimal> ExecuteAsync(ComputePremiumCommand command, CancellationToken cancellationToken = default);
}
