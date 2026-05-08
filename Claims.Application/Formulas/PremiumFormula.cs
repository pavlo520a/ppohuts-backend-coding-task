using Claims.Application.Abstractions.Formulas;
using Claims.Application.Extensions.Formulas;
using Claims.Application.Options.Formulas;
using Claims.Domain.Models.Formulas;
using Microsoft.Extensions.Options;

namespace Claims.Application.Formulas;

public sealed class PremiumFormula(IOptions<PremiumPricingOptions> options) : IFormula<CoverPremiumFormulaArgs>
{
    private readonly PremiumPricingOptions premiumPricing = options.Value;

    public decimal Calculate(CoverPremiumFormulaArgs args)
    {
        var totalDays = (args.EndDate.Date - args.StartDate.Date).Days + 1;

        if (totalDays <= 0)
        {
            return 0m;
        }

        var dailyPremium = premiumPricing.BaseDayRate * premiumPricing.TypeMultipliers.GetTypeMultiplier(args.CoverType);
        var remainingDays = totalDays;
        var totalPremium = 0m;

        foreach (var rule in premiumPricing.DiscountRules.DayRangeDiscountRules.TakeWhile(_ => remainingDays > 0))
        {
            var daysPerRule = rule.GetDaysPerRule(remainingDays);
            var discount = rule.GetDiscount(args.CoverType);

            totalPremium += daysPerRule * dailyPremium * (1m - discount);
            remainingDays -= daysPerRule;
        }

        if (remainingDays > 0)
        {
            var remainingDiscount = premiumPricing.DiscountRules.Remaining.GetDiscount(args.CoverType);
            totalPremium += remainingDays * dailyPremium * (1m - remainingDiscount);
        }

        return totalPremium;
    }
}
