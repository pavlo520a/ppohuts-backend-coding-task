using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Application.Abstractions.Formulas;
using Claims.Application.Commands.Covers;
using Claims.Application.UseCases.Covers;
using Claims.Data.Abstractions;
using Claims.Data.Abstractions.Repositories;
using Claims.Domain.Models;
using Claims.Domain.Models.Formulas;
using NSubstitute;

namespace Claims.Application.UnitTests.UseCases.Covers;

public class CreateCoverUseCaseTests
{
    private readonly Fixture _fixture = new();

    public CreateCoverUseCaseTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task ExecuteAsync_Should_WriteCoverAndAuditOutbox()
    {
        // Arrange
        var startOffsetDays = 1;
        var endOffsetDays = 10;
        var expectedPremium = 123.45m;

        var unitOfWork = _fixture.Freeze<IUnitOfWork>();
        var coverRepository = _fixture.Freeze<ICoverRepository>();
        var outboxRepository = _fixture.Freeze<IOutboxRepository>();
        var premiumFormula = _fixture.Freeze<IFormula<CoverPremiumFormulaArgs>>();

        var command = _fixture.Build<CreateCoverCommand>()
            .With(x => x.StartDate, DateTime.UtcNow.Date.AddDays(startOffsetDays))
            .With(x => x.EndDate, DateTime.UtcNow.Date.AddDays(endOffsetDays))
            .With(x => x.HttpMethod, TestConstants.HttpMethods.Post)
            .Create();

        var sut = _fixture.Create<CreateCoverUseCase>();

        premiumFormula.Calculate(Arg.Any<CoverPremiumFormulaArgs>())
            .Returns(expectedPremium);

        unitOfWork.CoversRepository
            .Returns(coverRepository);

        unitOfWork.OutboxRepository
            .Returns(outboxRepository);

        // Act
        await sut.ExecuteAsync(command, TestContext.Current.CancellationToken);

        // Assert
        coverRepository
            .Received(1)
            .Add(Arg.Is<Cover>(x => x.Premium == expectedPremium));

        outboxRepository
            .Received(1)
            .Add(Arg.Is<AuditOutbox>(x =>
                x.EntityType == nameof(Cover) &&
                x.HttpMethod == command.HttpMethod));

        await unitOfWork
            .Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
