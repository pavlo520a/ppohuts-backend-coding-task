using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Commands.Covers;
using Claims.Application.UnitTests.Factories;
using Claims.Application.Validation.Covers;
using Claims.Domain.Enums;

namespace Claims.Application.UnitTests.Validation.Covers;

public class CreateCoverCommandValidatorTests
{
    private readonly Fixture _fixture = new();

    public CreateCoverCommandValidatorTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ValidateAsync_Should_ReturnError_WhenStartDateIsInPast()
    {
        // Arrange
        var pastOffsetDays = -1;
        var endOffsetDays = 3;

        var command = _fixture.Build<CreateCoverCommand>()
            .With(x => x.StartDate, DateTime.UtcNow.Date.AddDays(pastOffsetDays))
            .With(x => x.EndDate, DateTime.UtcNow.Date.AddDays(endOffsetDays))
            .With(x => x.Type, CoverType.Yacht)
            .With(x => x.HttpMethod, TestConstants.HttpMethods.Post)
            .Create();

        var sut = new CreateCoverCommandValidator(ValidationRulesOptionsFactory.CreateRulesOptions());

        // Act
        var result = await sut.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.StartDate));
    }

    [Fact]
    public async Task ValidateAsync_Should_ReturnError_WhenPeriodExceedsMaxYears()
    {
        // Arrange
        var startOffsetDays = 1;
        var startDate = DateTime.UtcNow.Date.AddDays(startOffsetDays);

        var command = _fixture.Build<CreateCoverCommand>()
            .With(x => x.StartDate, startDate)
            .With(x => x.EndDate, startDate.AddYears(1))
            .With(x => x.Type, CoverType.Yacht)
            .With(x => x.HttpMethod, TestConstants.HttpMethods.Post)
            .Create();

        var sut = new CreateCoverCommandValidator(ValidationRulesOptionsFactory.CreateRulesOptions());

        // Act
        var result = await sut.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(result.Errors, x => x.ErrorMessage.Contains("cannot exceed 1 year"));
    }
}
