using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options.Formulas;

public sealed class PremiumPricingOptions
{
    public const string SectionName = "PremiumPricing";

    [Range(0.01, double.MaxValue)]
    public decimal BaseDayRate { get; init; }

    [Required]
    public PremiumTypeMultipliersOptions TypeMultipliers { get; init; } = new();

    [Required]
    public PremiumPricingDiscountRulesOptions DiscountRules { get; init; } = new();
}
