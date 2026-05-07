using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Commands.Claims;
using Claims.Application.UnitTests.Factories;
using Claims.Application.Validation.Claims;
using Claims.Data.Abstractions.Queries;
using Claims.Domain.Models;
using NSubstitute;

namespace Claims.Application.UnitTests.Validation.Claims;

public class CreateClaimCommandValidatorTests
{
    private readonly Fixture _fixture = new();

    public CreateClaimCommandValidatorTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ValidateAsync_Should_ReturnError_WhenCoverDoesNotExist()
    {
        // Arrange
        var missingCoverId = Guid.NewGuid().ToString();
        var coverQuery = _fixture.Freeze<ICoverQuery>();

        var command = _fixture.Build<CreateClaimCommand>()
            .With(x => x.CoverId, missingCoverId)
            .With(x => x.Created, DateTime.UtcNow.Date)
            .Create();

        var sut = new CreateClaimCommandValidator(
            coverQuery,
            ValidationRulesOptionsFactory.CreateRulesOptions());

        coverQuery.GetByIdAsync(command.CoverId, Arg.Any<CancellationToken>())
            .Returns((Cover?)null);

        // Act
        var result = await sut.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.CoverId));
    }

    [Fact]
    public async Task ValidateAsync_Should_ReturnError_WhenDamageCostExceedsLimit()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        var coverDurationDays = 5;
        var createdOffsetDays = 1;
        var damageCostAboveLimit = 100001m;
        var coverQuery = _fixture.Freeze<ICoverQuery>();

        var cover = _fixture.Build<Cover>()
            .With(x => x.Id, coverId)
            .With(x => x.StartDate, DateTime.UtcNow.Date)
            .With(x => x.EndDate, DateTime.UtcNow.Date.AddDays(coverDurationDays))
            .Create();

        var command = _fixture.Build<CreateClaimCommand>()
            .With(x => x.CoverId, coverId)
            .With(x => x.DamageCost, damageCostAboveLimit)
            .With(x => x.Created, DateTime.UtcNow.Date.AddDays(createdOffsetDays))
            .Create();

        var sut = new CreateClaimCommandValidator(
            coverQuery,
            ValidationRulesOptionsFactory.CreateRulesOptions());

        coverQuery.GetByIdAsync(command.CoverId, Arg.Any<CancellationToken>())
            .Returns(cover);

        // Act
        var result = await sut.ValidateAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.DamageCost));
    }
}
