using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options.Formulas;

public sealed class DayRangeDiscountRuleOptions
{
    [Required]
    public DayRangeOptions Days { get; init; } = new();

    [Range(0, 1)]
    public decimal YachtDiscount { get; init; }

    [Range(0, 1)]
    public decimal OtherDiscount { get; init; }
}
