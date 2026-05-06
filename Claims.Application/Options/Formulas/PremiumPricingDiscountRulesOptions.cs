using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options.Formulas;

public sealed class PremiumPricingDiscountRulesOptions
{
    [Required]
    public List<DayRangeDiscountRuleOptions> DayRangeDiscountRules { get; init; } = [];

    [Required]
    public DiscountRuleOptions Remaining { get; init; } = new();
}
