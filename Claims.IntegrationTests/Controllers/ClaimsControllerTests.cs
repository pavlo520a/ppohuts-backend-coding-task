using Claims.ApiModels.Claims;
using Claims.Domain.Enums;
using Claims.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace Claims.IntegrationTests;

public sealed class ClaimsControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task GetAllAsync_Should_ReturnAllClaims()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        var firstClaimId = Guid.NewGuid().ToString();
        var secondClaimId = Guid.NewGuid().ToString();

        await SeedCoverAsync(CreateCoverDocument(coverId));
        await SeedClaimAsync(CreateClaimDocument(firstClaimId, coverId, name: "Claim one"));
        await SeedClaimAsync(CreateClaimDocument(secondClaimId, coverId, name: "Claim two"));

        // Act
        var response = await Client.GetAsync("/claims", TestContext.Current.CancellationToken);

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

        await SeedCoverAsync(CreateCoverDocument(coverId));
        await SeedClaimAsync(CreateClaimDocument(claimId, coverId, name: "Lookup claim"));

        // Act
        var response = await Client.GetAsync($"/claims/{claimId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var claim = await response.ReadAsAsync<ClaimResponse>();

        Assert.Equal(claimId, claim.Id);
        Assert.Equal("Lookup claim", claim.Name);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNotFound_WhenClaimMissing()
    {
        // Arrange
        var missingId = Guid.NewGuid().ToString();

        // Act
        var response = await Client.GetAsync($"/claims/{missingId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_Should_CreateClaimAndOutboxMessage_WhenPayloadValid()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();

        await SeedCoverAsync(CreateCoverDocument(coverId));

        var request = TestDataFactory.CreateValidClaimRequest(
            coverId: coverId,
            created: DateTime.UtcNow.Date.AddDays(2),
            type: ClaimType.Fire);

        // Act
        var response = await Client.PostAsJsonAsync(
            "/claims",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.ReadAsAsync<ClaimResponse>();

        Assert.Equal(coverId, created.CoverId);
        Assert.Equal(ClaimType.Fire, created.Type);

        var outboxMessages = await GetOutboxMessagesAsync();

        Assert.Single(outboxMessages);
        Assert.Equal(OutboxMessageStatus.Pending, outboxMessages.Single().Status);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnBadRequest_WhenCoverDoesNotExist()
    {
        // Arrange
        var missingCoverId = Guid.NewGuid().ToString();

        var request = TestDataFactory.CreateValidClaimRequest(
            coverId: missingCoverId,
            created: DateTime.UtcNow.Date.AddDays(2));

        // Act
        var response = await Client.PostAsJsonAsync(
            "/claims",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("Related cover does not exist.", payload);
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveClaimAndCreateOutboxMessage_WhenClaimExists()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        var claimId = Guid.NewGuid().ToString();

        await SeedCoverAsync(CreateCoverDocument(coverId));
        await SeedClaimAsync(CreateClaimDocument(claimId, coverId));

        // Act
        var response = await Client.DeleteAsync($"/claims/{claimId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var claimExists = await ClaimExistsAsync(claimId);

        Assert.False(claimExists);

        var outboxMessages = await GetOutboxMessagesAsync();

        Assert.Single(outboxMessages);
        Assert.Equal(OutboxMessageStatus.Pending, outboxMessages.Single().Status);
    }
}
