using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Abstractions.Formulas;
using Claims.Application.Commands.Formulas;
using Claims.Application.UseCases.Formulas;
using Claims.Domain.Models.Formulas;
using NSubstitute;

namespace Claims.Application.UnitTests.UseCases.Formulas;

public class ComputePremiumUseCaseTests
{
    private readonly Fixture _fixture = new();

    public ComputePremiumUseCaseTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ExecuteAsync_Should_ReturnCalculatedPremium()
    {
        // Arrange
        var expectedPremium = 777m;

        var premiumFormula = _fixture.Freeze<IFormula<CoverPremiumFormulaArgs>>();
        var command = _fixture.Create<ComputePremiumCommand>();
        var sut = _fixture.Create<ComputePremiumUseCase>();

        premiumFormula.Calculate(Arg.Any<CoverPremiumFormulaArgs>())
            .Returns(expectedPremium);

        // Act
        var result = await sut.ExecuteAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedPremium, result);
    }
}
