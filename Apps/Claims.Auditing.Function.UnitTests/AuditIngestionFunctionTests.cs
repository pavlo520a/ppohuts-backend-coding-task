using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Claims.Auditing.AzureFunctionIsolated;
using Claims.Data.Auditing.Abstractions.Repositories;
using Claims.Domain.Models;
using NSubstitute;

namespace Claims.Auditing.AzureFunctionIsolated.UnitTests;

public class AuditIngestionFunctionTests
{
    private readonly Fixture _fixture = new();

    public AuditIngestionFunctionTests()
    {
        _fixture.Customize(new AutoNSubstituteCustomization
        {
            ConfigureMembers = true
        });
    }

    [Fact]
    public async Task RunAsync_Should_AddClaimAudit_WhenClaimAuditIsNew()
    {
        // Arrange
        var claimId = Guid.NewGuid().ToString();

        var claimAuditRepository = _fixture.Freeze<IClaimAuditTrailRepository>();
        var coverAuditRepository = _fixture.Freeze<ICoverAuditTrailRepository>();

        var payload = CreatePayload(
            nameof(Claim),
            claimId,
            TestConstants.HttpMethods.Post);

        var sut = _fixture.Create<AuditIngestionFunction>();

        claimAuditRepository.AnyAsync(claimId, TestConstants.HttpMethods.Post, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await sut.RunAsync(payload, TestContext.Current.CancellationToken);

        // Assert
        await claimAuditRepository
            .Received(1)
            .AddAsync(claimId, TestConstants.HttpMethods.Post, Arg.Any<CancellationToken>());

        await coverAuditRepository.DidNotReceive().AddAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_Should_NotAddClaimAudit_WhenDuplicate()
    {
        // Arrange
        var claimId = Guid.NewGuid().ToString();

        var claimAuditRepository = _fixture.Freeze<IClaimAuditTrailRepository>();

        var payload = CreatePayload(
            nameof(Claim),
            claimId,
            TestConstants.HttpMethods.Post);

        var sut = _fixture.Create<AuditIngestionFunction>();

        claimAuditRepository.AnyAsync(claimId, TestConstants.HttpMethods.Post, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await sut.RunAsync(payload, TestContext.Current.CancellationToken);

        // Assert
        await claimAuditRepository.DidNotReceive().AddAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_Should_AddCoverAudit_WhenCoverAuditIsNew()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();

        var claimAuditRepository = _fixture.Freeze<IClaimAuditTrailRepository>();
        var coverAuditRepository = _fixture.Freeze<ICoverAuditTrailRepository>();

        var payload = CreatePayload(
            nameof(Cover),
            coverId,
            TestConstants.HttpMethods.Delete);

        var sut = _fixture.Create<AuditIngestionFunction>();

        coverAuditRepository.AnyAsync(coverId, TestConstants.HttpMethods.Delete, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await sut.RunAsync(payload, TestContext.Current.CancellationToken);

        // Assert
        await coverAuditRepository
            .Received(1)
            .AddAsync(coverId, TestConstants.HttpMethods.Delete, Arg.Any<CancellationToken>());

        await claimAuditRepository.DidNotReceive().AddAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_Should_ThrowInvalidOperationException_WhenEntityTypeUnknown()
    {
        // Arrange
        var unknownEntityType = "UnknownEntity";
        var entityId = Guid.NewGuid().ToString();

        var payload = CreatePayload(
            unknownEntityType,
            entityId,
            TestConstants.HttpMethods.Post);

        var sut = _fixture.Create<AuditIngestionFunction>();

        // Act
        var action = () => sut.RunAsync(payload, TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    [Fact]
    public async Task RunAsync_Should_ThrowInvalidOperationException_WhenPayloadInvalid()
    {
        // Arrange
        var sut = _fixture.Create<AuditIngestionFunction>();

        // Act
        var action = () => sut.RunAsync("null", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    private static string CreatePayload(string entityType, string entityId, string httpMethod)
    {
        var outbox = new AuditOutbox
        {
            EntityType = entityType,
            EntityId = entityId,
            HttpMethod = httpMethod,
            OccurredAtUtc = DateTime.UtcNow
        };

        return JsonSerializer.Serialize(outbox);
    }
}
