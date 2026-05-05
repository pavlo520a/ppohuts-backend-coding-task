using Claims.Application.Formulas;
using Claims.Application.Options.Formulas;
using Claims.Domain.Enums;
using Claims.Domain.Models.Formulas;
using Microsoft.Extensions.Options;

namespace Claims.Application.UnitTests.Formulas;

public sealed class PremiumFormulaTests
{
    private static readonly DateTime StartDate = new(2026, 1, 1);
    private readonly PremiumFormula formula = new(CreatePricingOptions());

    [Theory]
    [InlineData(CoverType.Yacht, 1375)]
    [InlineData(CoverType.PassengerShip, 1500)]
    [InlineData(CoverType.Tanker, 1875)]
    [InlineData(CoverType.ContainerShip, 1625)]
    public void Calculate_AppliesTypeMultiplier_ForSingleInclusiveDay(CoverType coverType, decimal expected)
    {
        var premium = formula.Calculate(CreateArgs(coverType, StartDate, StartDate));

        Assert.Equal(expected, premium);
    }

    [Fact]
    public void Calculate_ComputesFirst30Days_WithoutDiscount()
    {
        var premium = formula.Calculate(CreateArgs(CoverType.Yacht, StartDate, StartDate.AddDays(29)));

        Assert.Equal(41250m, premium);
    }

    [Fact]
    public void Calculate_ComputesDay31_WithSecondTierDiscountForYacht()
    {
        var premium = formula.Calculate(CreateArgs(CoverType.Yacht, StartDate, StartDate.AddDays(30)));

        Assert.Equal(42556.25m, premium);
    }

    [Fact]
    public void Calculate_Computes180Days_WithSecondTierDiscountForNonYacht()
    {
        var premium = formula.Calculate(CreateArgs(CoverType.ContainerShip, StartDate, StartDate.AddDays(179)));

        Assert.Equal(287625m, premium);
    }

    [Fact]
    public void Calculate_Computes181Days_WithThirdTierDiscountForNonYacht()
    {
        var premium = formula.Calculate(CreateArgs(CoverType.ContainerShip, StartDate, StartDate.AddDays(180)));

        Assert.Equal(289201.25m, premium);
    }

    [Fact]
    public void Calculate_AppliesThirdTierBeyond365Days()
    {
        var premium = formula.Calculate(CreateArgs(CoverType.Yacht, StartDate, StartDate.AddDays(499)));

        Assert.Equal(641987.5m, premium);
    }

    [Fact]
    public void Calculate_ReturnsZero_WhenEndDateIsBeforeStartDate()
    {
        var premium = formula.Calculate(CreateArgs(CoverType.Yacht, StartDate, StartDate.AddDays(-1)));

        Assert.Equal(0m, premium);
    }

    private static CoverPremiumFormulaArgs CreateArgs(CoverType coverType, DateTime startDate, DateTime endDate)
    {
        return new CoverPremiumFormulaArgs
        {
            CoverType = coverType,
            StartDate = startDate,
            EndDate = endDate
        };
    }

    private static IOptions<PremiumPricingOptions> CreatePricingOptions()
    {
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
            PricingRules =
            [
                new PremiumPricingRuleOptions
                {
                    Days = new PremiumPricingDaysRangeOptions
                    {
                        From = 1,
                        To = 30
                    },
                    YachtDiscount = 0m,
                    OtherDiscount = 0m
                },
                new PremiumPricingRuleOptions
                {
                    Days = new PremiumPricingDaysRangeOptions
                    {
                        From = 31,
                        To = 180
                    },
                    YachtDiscount = 0.05m,
                    OtherDiscount = 0.02m
                },
                new PremiumPricingRuleOptions
                {
                    Days = new PremiumPricingDaysRangeOptions(),
                    YachtDiscount = 0.08m,
                    OtherDiscount = 0.03m
                }
            ]
        });
    }
}
