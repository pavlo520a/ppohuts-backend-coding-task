using Claims.Application.Options.Formulas;
using Claims.Domain.Enums;

namespace Claims.Application.Extensions.Formulas;

public static class DiscountRuleExtensions
{
    public static decimal GetDiscount(this DiscountRuleOptions rule, CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => rule.YachtDiscount,
            _ => rule.OtherDiscount
        };
    }

    public static decimal GetDiscount(this DayRangeDiscountRuleOptions rule, CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => rule.YachtDiscount,
            _ => rule.OtherDiscount
        };
    }
}
