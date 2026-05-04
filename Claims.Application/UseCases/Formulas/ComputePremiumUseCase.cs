using Claims.Application.Abstractions;
using Claims.Application.Abstractions.Formulas;
using Claims.Application.Commands.Formulas;
using Claims.Domain.Models.Formulas;

namespace Claims.Application.UseCases.Formulas;

public sealed class ComputePremiumUseCase(IFormula<CoverPremiumFormulaArgs> premiumFormula)
    : IUseCase<ComputePremiumCommand, decimal>
{
    public Task<decimal> ExecuteAsync(ComputePremiumCommand command, CancellationToken _)
    {
        var args = new CoverPremiumFormulaArgs
        {
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            CoverType = command.CoverType
        };

        return Task.FromResult(premiumFormula.Calculate(args));
    }
}
