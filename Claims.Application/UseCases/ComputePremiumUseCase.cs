using Claims.Application.Commands;
using Claims.Application.Premium;

namespace Claims.Application.UseCases;

public sealed class ComputePremiumUseCase(IPremiumCalculator premiumCalculator) : IComputePremiumUseCase
{
    public Task<decimal> ExecuteAsync(ComputePremiumCommand command, CancellationToken cancellationToken) =>
        Task.FromResult(premiumCalculator.Compute(command.StartDate, command.EndDate, command.CoverType));
}
