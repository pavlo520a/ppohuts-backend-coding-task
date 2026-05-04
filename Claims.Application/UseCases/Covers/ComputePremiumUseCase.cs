using Claims.Application.Abstractions;
using Claims.Application.Abstractions.Formulas;
using Claims.Application.Commands.Covers;
using Claims.Domain.Models.Formulas;

namespace Claims.Application.UseCases.Covers;

public sealed class ComputePremiumUseCase(IFormula<CoverPremiumFormulaArgs> premiumFormula)
    : IUseCase<ComputePremiumCommand, decimal>
{
    public decimal Execute(ComputePremiumCommand command)
    {
        var args = new CoverPremiumFormulaArgs(command.StartDate, command.EndDate, command.CoverType);
        return premiumFormula.Calculate(args);
    }
}
