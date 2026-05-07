using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Commands.Covers;
using Claims.Application.UseCases.Covers;
using Claims.Data.Abstractions.Queries;
using Claims.Domain.Exceptions;
using Claims.Domain.Models;
using NSubstitute;

namespace Claims.Application.UnitTests.UseCases.Covers;

public class GetCoverByIdUseCaseTests
{
    private readonly Fixture _fixture = new();

    public GetCoverByIdUseCaseTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ExecuteAsync_Should_ReturnCover_WhenFound()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        var coverQuery = _fixture.Freeze<ICoverQuery>();

        var command = _fixture.Build<GetCoverByIdCommand>()
            .With(x => x.Id, coverId)
            .Create();

        var expected = _fixture.Create<Cover>();
        var sut = _fixture.Create<GetCoverByIdUseCase>();

        coverQuery.GetByIdAsync(coverId, Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await sut.ExecuteAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected, result);
    }

    [Fact]
    public async Task ExecuteAsync_Should_ThrowCoverNotFoundException_WhenMissing()
    {
        // Arrange
        var missingCoverId = Guid.NewGuid().ToString();
        var coverQuery = _fixture.Freeze<ICoverQuery>();

        var command = _fixture.Build<GetCoverByIdCommand>()
            .With(x => x.Id, missingCoverId)
            .Create();

        var sut = _fixture.Create<GetCoverByIdUseCase>();

        coverQuery.GetByIdAsync(missingCoverId, Arg.Any<CancellationToken>())
            .Returns((Cover?)null);

        // Act
        var action = () => sut.ExecuteAsync(command, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<CoverNotFoundException>(action);
    }
}
