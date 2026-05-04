using Claims.ApiModels.Formulas;
using Claims.Application.Commands.Formulas;

namespace Claims.Mapping;

public static class FormulaApiMapping
{
    public static ComputePremiumCommand ToCommand(this ComputePremiumRequest request, string httpMethod) =>
        new()
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CoverType = request.CoverType,
            HttpMethod = httpMethod
        };
}
