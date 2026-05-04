using Claims.Domain.Enums;

namespace Claims.Domain.Models.Formulas;

public sealed record CoverPremiumFormulaArgs(
    DateTime StartDate,
    DateTime EndDate,
    CoverType CoverType);
