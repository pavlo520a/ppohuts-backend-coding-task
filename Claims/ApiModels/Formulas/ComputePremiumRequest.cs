using Claims.Domain.Enums;

namespace Claims.ApiModels.Formulas;

public sealed class ComputePremiumRequest
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public CoverType CoverType { get; set; }
}
