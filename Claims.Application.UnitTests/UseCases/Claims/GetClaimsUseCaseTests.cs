using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Commands.Claims;
using Claims.Application.UseCases.Claims;
using Claims.Data.Abstractions.Queries;
using Claims.Domain.Models;
using NSubstitute;

namespace Claims.Application.UnitTests.UseCases.Claims;

public class GetClaimsUseCaseTests
{
    private readonly Fixture _fixture = new();

    public GetClaimsUseCaseTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ExecuteAsync_Should_ReturnAllClaims()
    {
        // Arrange
        var claimsCount = 2;
        var expected = _fixture.CreateMany<Claim>(claimsCount)
            .ToList();

        var claimQuery = _fixture.Freeze<IClaimQuery>();
        var command = _fixture.Create<GetClaimsCommand>();

        claimQuery.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(expected);

        var sut = _fixture.Create<GetClaimsUseCase>();

        // Act
        var result = await sut.ExecuteAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected, result);
    }
}
