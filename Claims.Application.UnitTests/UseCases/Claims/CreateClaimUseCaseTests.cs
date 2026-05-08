using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Commands.Claims;
using Claims.Application.UseCases.Claims;
using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Models;
using NSubstitute;

namespace Claims.Application.UnitTests.UseCases.Claims;

public class CreateClaimUseCaseTests
{
    private readonly Fixture _fixture = new();

    public CreateClaimUseCaseTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ExecuteAsync_Should_WriteClaimAndAuditOutbox()
    {
        // Arrange
        var unitOfWork = _fixture.Freeze<IUnitOfWork>();
        var claimRepository = _fixture.Freeze<IClaimRepository>();
        var outboxRepository = _fixture.Freeze<IOutboxRepository>();

        var command = _fixture.Build<CreateClaimCommand>()
            .With(x => x.HttpMethod, TestConstants.HttpMethods.Post)
            .Create();

        var sut = _fixture.Create<CreateClaimUseCase>();

        unitOfWork.ClaimsRepository
            .Returns(claimRepository);

        unitOfWork.OutboxRepository
            .Returns(outboxRepository);

        // Act
        await sut.ExecuteAsync(command, TestContext.Current.CancellationToken);

        // Assert
        claimRepository
            .Received(1)
            .Add(Arg.Any<Claim>());

        outboxRepository
            .Received(1)
            .Add(Arg.Is<AuditOutbox>(x =>
                x.EntityType == nameof(Claim) &&
                x.HttpMethod == command.HttpMethod));

        await unitOfWork
            .Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
