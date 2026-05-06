using System.ComponentModel.DataAnnotations;

namespace Claims.Application.Options.Formulas;

public sealed class DiscountRuleOptions
{
    [Range(0, 1)]
    public decimal YachtDiscount { get; init; }

    [Range(0, 1)]
    public decimal OtherDiscount { get; init; }
}
