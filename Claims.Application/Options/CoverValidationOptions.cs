using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options;

public sealed class CoverValidationOptions
{
    [Range(1, int.MaxValue)]
    public int MaxInsurancePeriodYears { get; init; } = 1;
}
