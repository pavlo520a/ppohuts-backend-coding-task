using Claims.ApiModels.Claims;
using Claims.Domain.Enums;
using Claims.IntegrationTests.Infrastructure;
using Claims.IntegrationTests.Infrastructure.Constants;
using Claims.IntegrationTests.Infrastructure.Factories;
using Claims.IntegrationTests.Infrastructure.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;

namespace Claims.IntegrationTests.Controllers;

public sealed class ClaimsControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task GetAllAsync_Should_ReturnAllClaims()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        var firstClaimId = Guid.NewGuid().ToString();
        var secondClaimId = Guid.NewGuid().ToString();
        var firstClaimName = "Claim one";
        var secondClaimName = "Claim two";

        await DataStore.SeedCoverAsync(TestDocumentFactory.CreateCoverDocument(coverId));
        await DataStore.SeedClaimAsync(TestDocumentFactory.CreateClaimDocument(firstClaimId, coverId, name: firstClaimName));
        await DataStore.SeedClaimAsync(TestDocumentFactory.CreateClaimDocument(secondClaimId, coverId, name: secondClaimName));

        // Act
        var response = await Client.GetAsync(TestConstants.Routes.Claims, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var claims = await response.ReadAsAsync<IReadOnlyList<ClaimResponse>>();

        Assert.Equal(2, claims.Count);
        Assert.Contains(claims, x => x.Id == firstClaimId);
        Assert.Contains(claims, x => x.Id == secondClaimId);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnClaim_WhenClaimExists()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        var claimId = Guid.NewGuid().ToString();
        var claimName = "Lookup claim";

        await DataStore.SeedCoverAsync(TestDocumentFactory.CreateCoverDocument(coverId));
        await DataStore.SeedClaimAsync(TestDocumentFactory.CreateClaimDocument(claimId, coverId, name: claimName));

        // Act
        var response = await Client.GetAsync($"{TestConstants.Routes.Claims}/{claimId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var claim = await response.ReadAsAsync<ClaimResponse>();

        Assert.Equal(claimId, claim.Id);
        Assert.Equal(claimName, claim.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNotFound_WhenClaimMissing()
    {
        // Arrange
        var missingId = Guid.NewGuid().ToString();

        // Act
        var response = await Client.GetAsync($"{TestConstants.Routes.Claims}/{missingId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_Should_CreateClaimAndOutboxMessage_WhenPayloadValid()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();

        await DataStore.SeedCoverAsync(TestDocumentFactory.CreateCoverDocument(coverId));

        var request = TestDataFactory.CreateClaimRequest(
            coverId: coverId,
            created: DateTime.UtcNow.Date.AddDays(2),
            type: ClaimType.Fire);

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.Claims,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.ReadAsAsync<ClaimResponse>();

        Assert.Equal(coverId, created.CoverId);
        Assert.Equal(ClaimType.Fire, created.Type);

        var outboxMessages = await DataStore.GetOutboxMessagesAsync();

        Assert.Single(outboxMessages);
        Assert.Equal(OutboxMessageStatus.Pending, outboxMessages.Single().Status);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnBadRequest_WhenCoverDoesNotExist()
    {
        // Arrange
        var missingCoverId = Guid.NewGuid().ToString();

        var request = TestDataFactory.CreateClaimRequest(
            coverId: missingCoverId,
            created: DateTime.UtcNow.Date.AddDays(2));

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.Claims,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("Related cover does not exist.", payload);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnBadRequest_WhenCoverIdIsEmpty()
    {
        // Arrange
        var request = TestDataFactory.CreateClaimRequest(coverId: string.Empty);

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.Claims,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnBadRequest_WhenDamageCostExceedsMaxRule()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        await DataStore.SeedCoverAsync(TestDocumentFactory.CreateCoverDocument(coverId));

        var request = TestDataFactory.CreateClaimRequest(
            coverId: coverId,
            damageCost: Configuration.GetValue<decimal>("ValidationRules:Claims:MaxDamageCost") + 1m);

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.Claims,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("DamageCost cannot exceed", payload);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnBadRequest_WhenCreatedDateOutsideCoverPeriod()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        var cover = TestDocumentFactory.CreateCoverDocument(
            coverId,
            startDate: DateTime.UtcNow.Date.AddDays(10),
            endDate: DateTime.UtcNow.Date.AddDays(20));

        await DataStore.SeedCoverAsync(cover);

        var request = TestDataFactory.CreateClaimRequest(
            coverId: coverId,
            created: DateTime.UtcNow.Date.AddDays(5));

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.Claims,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("Created date must be within the period of the related cover.", payload);
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveClaimAndCreateOutboxMessage_WhenClaimExists()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        var claimId = Guid.NewGuid().ToString();

        await DataStore.SeedCoverAsync(TestDocumentFactory.CreateCoverDocument(coverId));
        await DataStore.SeedClaimAsync(TestDocumentFactory.CreateClaimDocument(claimId, coverId));

        // Act
        var response = await Client.DeleteAsync($"{TestConstants.Routes.Claims}/{claimId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var claimExists = await DataStore.ClaimExistsAsync(claimId);

        Assert.False(claimExists);

        var outboxMessages = await DataStore.GetOutboxMessagesAsync();

        Assert.Single(outboxMessages);
        Assert.Equal(OutboxMessageStatus.Pending, outboxMessages.Single().Status);
    }
}
