using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Commands.Covers;
using Claims.Application.UseCases.Covers;
using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Models;
using NSubstitute;

namespace Claims.Application.UnitTests.UseCases.Covers;

public class DeleteCoverUseCaseTests
{
    private readonly Fixture _fixture = new();

    public DeleteCoverUseCaseTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ExecuteAsync_Should_DeleteCoverAndWriteAuditOutbox()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        var unitOfWork = _fixture.Freeze<IUnitOfWork>();
        var coverRepository = _fixture.Freeze<ICoverRepository>();
        var outboxRepository = _fixture.Freeze<IOutboxRepository>();

        var command = _fixture.Build<DeleteCoverCommand>()
            .With(x => x.Id, coverId)
            .With(x => x.HttpMethod, TestConstants.HttpMethods.Delete)
            .Create();

        var sut = _fixture.Create<DeleteCoverUseCase>();

        unitOfWork.CoversRepository
            .Returns(coverRepository);

        unitOfWork.OutboxRepository
            .Returns(outboxRepository);

        // Act
        await sut.ExecuteAsync(command, TestContext.Current.CancellationToken);

        // Assert
        coverRepository
            .Received(1)
            .Delete(coverId);

        outboxRepository
            .Received(1)
            .Add(Arg.Is<AuditOutbox>(x =>
                x.EntityType == nameof(Cover) &&
                x.EntityId == coverId &&
                x.HttpMethod == command.HttpMethod));

        await unitOfWork
            .Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
