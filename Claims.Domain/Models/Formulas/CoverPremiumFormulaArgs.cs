using Claims.Domain.Enums;

namespace Claims.Domain.Models.Formulas;

public class CoverPremiumFormulaArgs
{
    public required DateTime StartDate { get; set; }

    public required DateTime EndDate { get; set; }

    public required CoverType CoverType { get; set; }
}