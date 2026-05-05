using Claims.Application.Options.Formulas;

namespace Claims.Application.Extensions.Formulas;

public static class DayRangeDiscountRuleExtensions
{
    public static int GetDaysForRule(this DayRangeDiscountRuleOptions rule, int remainingDays)
    {
        if (rule.Days.From is not null && rule.Days.To is not null)
        {
            var rangeLength = rule.Days.To.Value - rule.Days.From.Value + 1;
            return Math.Max(0, Math.Min(remainingDays, rangeLength));
        }

        if (rule.Days.To is not null)
        {
            return Math.Max(0, Math.Min(remainingDays, rule.Days.To.Value));
        }

        return remainingDays;
    }
}
