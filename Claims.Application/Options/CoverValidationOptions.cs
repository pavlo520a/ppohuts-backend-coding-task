using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options;

public sealed class CoverValidationOptions
{
    [Range(1, 10)]
    public int MaxInsurancePeriodYears { get; init; } = 1;
}
