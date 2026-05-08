using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Formulas;
using Claims.Application.UnitTests.Factories;
using Claims.Domain.Enums;
using Claims.Domain.Models.Formulas;

namespace Claims.Application.UnitTests.Formulas;

public sealed class PremiumFormulaTests
{
    private static readonly DateTime StartDate = new(2026, 1, 1);
    private readonly Fixture _fixture = new();

    public PremiumFormulaTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public void Calculate_Should_ApplyYachtMultiplier_ForSingleInclusiveDay()
    {
        // Arrange
        var expectedPremium = 1375m;

        var sut = CreateSut();
        var formulaArgs = CreateArgs(CoverType.Yacht, StartDate, StartDate);

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_Should_ApplyPassengerShipMultiplier_ForSingleInclusiveDay()
    {
        // Arrange
        var expectedPremium = 1500m;

        var sut = CreateSut();
        var formulaArgs = CreateArgs(CoverType.PassengerShip, StartDate, StartDate);

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_Should_ApplyTankerMultiplier_ForSingleInclusiveDay()
    {
        // Arrange
        var expectedPremium = 1875m;

        var sut = CreateSut();
        var formulaArgs = CreateArgs(CoverType.Tanker, StartDate, StartDate);

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_Should_ApplyContainerShipMultiplier_ForSingleInclusiveDay()
    {
        // Arrange
        var expectedPremium = 1625m;

        var sut = CreateSut();
        var formulaArgs = CreateArgs(CoverType.ContainerShip, StartDate, StartDate);

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_Should_ComputeFirst30Days_WithoutDiscount()
    {
        // Arrange
        var firstTierLastDayOffset = 29;
        var expectedPremium = 41250m;

        var sut = CreateSut();
        var formulaArgs = CreateArgs(CoverType.Yacht, StartDate, StartDate.AddDays(firstTierLastDayOffset));

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_Should_ComputeDay31_WithSecondTierDiscountForYacht()
    {
        // Arrange
        var secondTierFirstDayOffset = 30;
        var expectedPremium = 42556.25m;

        var sut = CreateSut();
        var formulaArgs = CreateArgs(CoverType.Yacht, StartDate, StartDate.AddDays(secondTierFirstDayOffset));

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_Should_Compute180Days_WithSecondTierDiscountForNonYacht()
    {
        // Arrange
        var secondTierLastDayOffset = 179;
        var expectedPremium = 287625m;

        var sut = CreateSut();
        var formulaArgs = CreateArgs(CoverType.ContainerShip, StartDate, StartDate.AddDays(secondTierLastDayOffset));

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_Should_Compute181Days_WithThirdTierDiscountForNonYacht()
    {
        // Arrange
        var remainingTierFirstDayOffset = 180;
        var expectedPremium = 289201.25m;

        var sut = CreateSut();
        var formulaArgs = CreateArgs(CoverType.ContainerShip, StartDate, StartDate.AddDays(remainingTierFirstDayOffset));

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_Should_ApplyThirdTierBeyond365Days()
    {
        // Arrange
        var longRangeOffsetDays = 499;
        var expectedPremium = 641987.5m;

        var sut = CreateSut();
        var formulaArgs = CreateArgs(CoverType.Yacht, StartDate, StartDate.AddDays(longRangeOffsetDays));

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_Should_ReturnZero_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var endBeforeStartOffsetDays = -1;
        var expectedPremium = 0m;

        var sut = CreateSut();
        var formulaArgs = CreateArgs(CoverType.Yacht, StartDate, StartDate.AddDays(endBeforeStartOffsetDays));

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    [Fact]
    public void Calculate_Should_UseRemainingRule_WhenDayRangeDiscountRulesAreEmpty()
    {
        // Arrange
        var firstTierLastDayOffset = 29;
        var expectedPremium = 37950m;

        var options = PremiumPricingOptionsFactory.CreatePricingOptions(dayRangeDiscountRules: []);
        var sut = new PremiumFormula(options);
        var formulaArgs = CreateArgs(CoverType.Yacht, StartDate, StartDate.AddDays(firstTierLastDayOffset));

        // Act
        var premium = sut.Calculate(formulaArgs);

        // Assert
        Assert.Equal(expectedPremium, premium);
    }

    private CoverPremiumFormulaArgs CreateArgs(CoverType coverType, DateTime startDate, DateTime endDate)
    {
        return _fixture.Build<CoverPremiumFormulaArgs>()
            .With(x => x.CoverType, coverType)
            .With(x => x.StartDate, startDate)
            .With(x => x.EndDate, endDate)
            .Create();
    }

    private static PremiumFormula CreateSut()
    {
        return new PremiumFormula(PremiumPricingOptionsFactory.CreatePricingOptions());
    }
}
