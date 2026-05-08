using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Commands.Claims;
using Claims.Application.UseCases.Claims;
using Claims.Data.Abstractions.Queries;
using Claims.Domain.Exceptions;
using Claims.Domain.Models;
using NSubstitute;

namespace Claims.Application.UnitTests.UseCases.Claims;

public class GetClaimByIdUseCaseTests
{
    private readonly Fixture _fixture = new();

    public GetClaimByIdUseCaseTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ExecuteAsync_Should_ReturnClaim_WhenFound()
    {
        // Arrange
        var claimId = Guid.NewGuid().ToString();
        var claimQuery = _fixture.Freeze<IClaimQuery>();

        var command = _fixture.Build<GetClaimByIdCommand>()
            .With(x => x.Id, claimId)
            .Create();

        var expected = _fixture.Create<Claim>();
        var sut = _fixture.Create<GetClaimByIdUseCase>();

        claimQuery.GetByIdAsync(claimId, Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await sut.ExecuteAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(expected, result);
    }

    [Fact]
    public async Task ExecuteAsync_Should_ThrowClaimNotFoundException_WhenMissing()
    {
        // Arrange
        var missingClaimId = Guid.NewGuid().ToString();
        var claimQuery = _fixture.Freeze<IClaimQuery>();

        var command = _fixture.Build<GetClaimByIdCommand>()
            .With(x => x.Id, missingClaimId)
            .Create();

        var sut = _fixture.Create<GetClaimByIdUseCase>();

        claimQuery.GetByIdAsync(missingClaimId, Arg.Any<CancellationToken>())
            .Returns((Claim?)null);

        // Act
        var action = () => sut.ExecuteAsync(command, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<ClaimNotFoundException>(action);
    }
}
