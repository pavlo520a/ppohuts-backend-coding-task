using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options.Formulas;

public sealed class DayRangeDiscountRuleOptions : DiscountRuleOptions
{
    [Required]
    public DayRangeOptions Days { get; init; } = new();
}
