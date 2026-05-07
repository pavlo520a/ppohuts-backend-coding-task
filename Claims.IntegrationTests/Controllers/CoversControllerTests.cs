using Claims.ApiModels.Covers;
using Claims.Domain.Enums;
using Claims.IntegrationTests.Infrastructure;
using Claims.IntegrationTests.Infrastructure.Constants;
using Claims.IntegrationTests.Infrastructure.Factories;
using Claims.IntegrationTests.Infrastructure.Hosting;
using System.Net;
using System.Net.Http.Json;

namespace Claims.IntegrationTests.Controllers;

public sealed class CoversControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task GetAllAsync_Should_ReturnAllCovers()
    {
        // Arrange
        var firstCoverId = Guid.NewGuid().ToString();
        var secondCoverId = Guid.NewGuid().ToString();

        await Factory.DataStore.SeedCoverAsync(TestDocumentFactory.CreateCoverDocument(firstCoverId, premium: 1100m));
        await Factory.DataStore.SeedCoverAsync(TestDocumentFactory.CreateCoverDocument(secondCoverId, premium: 1300m));

        // Act
        var response = await Client.GetAsync(TestConstants.Routes.Covers, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var covers = await response.ReadAsAsync<IReadOnlyList<CoverResponse>>();

        Assert.Equal(2, covers.Count);
        Assert.Contains(covers, x => x.Id == firstCoverId);
        Assert.Contains(covers, x => x.Id == secondCoverId);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnCover_WhenCoverExists()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();

        await Factory.DataStore.SeedCoverAsync(TestDocumentFactory.CreateCoverDocument(coverId, premium: 2000m));

        // Act
        var response = await Client.GetAsync($"{TestConstants.Routes.Covers}/{coverId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var cover = await response.ReadAsAsync<CoverResponse>();

        Assert.Equal(coverId, cover.Id);
        Assert.Equal(2000m, cover.Premium);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNotFound_WhenCoverMissing()
    {
        // Arrange
        var missingId = Guid.NewGuid().ToString();

        // Act
        var response = await Client.GetAsync($"{TestConstants.Routes.Covers}/{missingId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_Should_CreateCoverAndOutboxMessage_WhenPayloadValid()
    {
        // Arrange
        var request = TestDataFactory.CreateValidCoverRequest(
            startDate: DateTime.UtcNow.Date.AddDays(1),
            endDate: DateTime.UtcNow.Date.AddDays(11),
            type: CoverType.Yacht);

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.Covers,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.ReadAsAsync<CoverResponse>();

        Assert.Equal(CoverType.Yacht, created.Type);
        Assert.True(created.Premium > 0m);

        var outboxMessages = await Factory.DataStore.GetOutboxMessagesAsync();

        Assert.Single(outboxMessages);
        Assert.Equal(OutboxMessageStatus.Pending, outboxMessages.Single().Status);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnBadRequest_WhenStartDateInPast()
    {
        // Arrange
        var request = TestDataFactory.CreateValidCoverRequest(
            startDate: DateTime.UtcNow.Date.AddDays(-1),
            endDate: DateTime.UtcNow.Date.AddDays(10));

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.Covers,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("StartDate cannot be in the past.", payload);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnBadRequest_WhenEndDateBeforeStartDate()
    {
        // Arrange
        var request = TestDataFactory.CreateValidCoverRequest(
            startDate: DateTime.UtcNow.Date.AddDays(10),
            endDate: DateTime.UtcNow.Date.AddDays(9));

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.Covers,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("EndDate must be greater than or equal to StartDate.", payload);
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnBadRequest_WhenInsurancePeriodExceedsMaxRule()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var request = TestDataFactory.CreateValidCoverRequest(
            startDate: startDate,
            endDate: startDate.AddYears(TestConstants.Validation.MaxCoverInsurancePeriodYears));

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.Covers,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("The total insurance period cannot exceed", payload);
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveCoverAndCreateOutboxMessage_WhenCoverExists()
    {
        // Arrange
        var coverId = Guid.NewGuid().ToString();
        await Factory.DataStore.SeedCoverAsync(TestDocumentFactory.CreateCoverDocument(coverId));

        // Act
        var response = await Client.DeleteAsync($"{TestConstants.Routes.Covers}/{coverId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var coverExists = await Factory.DataStore.CoverExistsAsync(coverId);

        Assert.False(coverExists);

        var outboxMessages = await Factory.DataStore.GetOutboxMessagesAsync();

        Assert.Single(outboxMessages);
        Assert.Equal(OutboxMessageStatus.Pending, outboxMessages.Single().Status);
    }
}
