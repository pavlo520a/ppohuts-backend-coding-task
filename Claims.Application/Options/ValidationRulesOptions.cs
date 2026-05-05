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
