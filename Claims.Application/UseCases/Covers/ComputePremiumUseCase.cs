using Claims.Application.Abstractions;
using Claims.Application.Abstractions.Formulas;
using Claims.Application.Commands.Covers;
using Claims.Domain.Models.Formulas;

namespace Claims.Application.UseCases.Covers;

public sealed class ComputePremiumUseCase(IFormula<CoverPremiumFormulaArgs> premiumFormula)
    : IUseCase<ComputePremiumCommand, decimal>
{
    public Task<decimal> ExecuteAsync(ComputePremiumCommand command, CancellationToken cancellationToken)
    {
        var args = new CoverPremiumFormulaArgs(command.StartDate, command.EndDate, command.CoverType);
        return Task.FromResult(premiumFormula.Calculate(args));
    }
}
