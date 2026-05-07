using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Commands.Covers;
using Claims.Application.UseCases.Covers;
using Claims.Data.Abstractions.Queries;
using Claims.Domain.Models;
using NSubstitute;

namespace Claims.Application.UnitTests.UseCases.Covers;

public class GetCoversUseCaseTests
{
    private readonly Fixture _fixture = new();

    public GetCoversUseCaseTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ExecuteAsync_Should_ReturnAllCovers()
    {
        // Arrange
        var coversCount = 2;

        var coverQuery = _fixture.Freeze<ICoverQuery>();
        var command = _fixture.Create<GetCoversCommand>();
        var expected = _fixture.CreateMany<Cover>(coversCount).ToList();

        var sut = _fixture.Create<GetCoversUseCase>();

        coverQuery.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await sut.ExecuteAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected, result);
    }
}
