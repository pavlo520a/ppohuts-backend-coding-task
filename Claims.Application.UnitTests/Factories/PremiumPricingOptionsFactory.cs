using Claims.Application.Options.Formulas;
using Microsoft.Extensions.Options;

namespace Claims.Application.UnitTests.Factories;

internal static class PremiumPricingOptionsFactory
{
    public static IOptions<PremiumPricingOptions> CreatePricingOptions(
        List<DayRangeDiscountRuleOptions>? dayRangeDiscountRules = null)
    {
        var defaultDayRangeDiscountRules = new List<DayRangeDiscountRuleOptions>
        {
            new()
            {
                Days = new DayRangeOptions
                {
                    From = 1,
                    To = 30
                },
                YachtDiscount = 0m,
                OtherDiscount = 0m
            },
            new()
            {
                Days = new DayRangeOptions
                {
                    From = 31,
                    To = 180
                },
                YachtDiscount = 0.05m,
                OtherDiscount = 0.02m
            }
        };

        return Microsoft.Extensions.Options.Options.Create(new PremiumPricingOptions
        {
            BaseDayRate = 1250m,
            TypeMultipliers = new PremiumTypeMultipliersOptions
            {
                Yacht = 1.1m,
                PassengerShip = 1.2m,
                Tanker = 1.5m,
                Other = 1.3m
            },
            DiscountRules = new PremiumPricingDiscountRulesOptions
            {
                DayRangeDiscountRules = dayRangeDiscountRules ?? defaultDayRangeDiscountRules,
                Remaining = new DiscountRuleOptions
                {
                    YachtDiscount = 0.08m,
                    OtherDiscount = 0.03m
                }
            }
        });
    }
}
