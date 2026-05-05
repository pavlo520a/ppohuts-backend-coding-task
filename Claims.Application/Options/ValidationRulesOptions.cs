using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options;

public sealed class ValidationRulesOptions
{
    public const string SectionName = "ValidationRules";

    [Required]
    public ClaimValidationOptions Claims { get; init; } = new();

    [Required]
    public CoverValidationOptions Covers { get; init; } = new();
}

public sealed class ClaimValidationOptions
{
    [Range(typeof(decimal), "0", "1000000000")]
    public decimal MaxDamageCost { get; init; } = 100000m;
}

public sealed class CoverValidationOptions
{
    [Range(1, 10)]
    public int MaxInsurancePeriodYears { get; init; } = 1;
}
