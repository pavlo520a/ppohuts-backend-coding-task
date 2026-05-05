using Claims.Application.Abstractions.Formulas;
using Claims.Application.Options.Formulas;
using Claims.Domain.Enums;
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

        var dailyPremium = premiumPricing.BaseDayRate * GetTypeMultiplier(args.CoverType);
        var remainingDays = totalDays;
        var totalPremium = 0m;

        foreach (var rule in premiumPricing.PricingRules)
        {
            if (remainingDays <= 0)
            {
                break;
            }

            var daysForRule = GetDaysForRule(rule, remainingDays);
            var discount = args.CoverType == CoverType.Yacht ? rule.YachtDiscount : rule.OtherDiscount;
            totalPremium += daysForRule * dailyPremium * (1m - discount);
            remainingDays -= daysForRule;
        }

        return totalPremium;
    }

    private decimal GetTypeMultiplier(CoverType coverType)
    {
        return coverType switch
        {
            CoverType.Yacht => premiumPricing.TypeMultipliers.Yacht,
            CoverType.PassengerShip => premiumPricing.TypeMultipliers.PassengerShip,
            CoverType.Tanker => premiumPricing.TypeMultipliers.Tanker,
            _ => premiumPricing.TypeMultipliers.Other
        };
    }

    private static int GetDaysForRule(PremiumPricingRuleOptions rule, int remainingDays)
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
