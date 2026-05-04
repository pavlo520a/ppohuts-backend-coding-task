using Claims.ApiModels.Formulas;
using Claims.Application.Commands.Formulas;

namespace Claims.Mapping;

public static class FormulaApiMapping
{
    public static ComputePremiumCommand ToCommand(this ComputePremiumRequest request) =>
        new(request.StartDate, request.EndDate, request.CoverType);
}
