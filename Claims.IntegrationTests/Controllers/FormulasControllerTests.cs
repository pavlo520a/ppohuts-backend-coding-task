using Claims.ApiModels.Formulas;
using Claims.Domain.Enums;
using Claims.IntegrationTests.Infrastructure;
using Claims.IntegrationTests.Infrastructure.Constants;
using Claims.IntegrationTests.Infrastructure.Factories;
using Claims.IntegrationTests.Infrastructure.Hosting;
using System.Net;
using System.Net.Http.Json;

namespace Claims.IntegrationTests.Controllers;

public sealed class FormulasControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task ComputePremiumAsync_Should_ReturnPremium_WhenPayloadValid()
    {
        // Arrange
        var request = TestDataFactory.CreateValidPremiumRequest(
            startDate: DateTime.UtcNow.Date.AddDays(1),
            endDate: DateTime.UtcNow.Date.AddDays(10),
            type: CoverType.Tanker);

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.ComputePremium,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var premium = await response.ReadAsAsync<ComputePremiumResponse>();
        Assert.True(premium.Amount > 0m);
    }

    [Fact]
    public async Task ComputePremiumAsync_Should_ReturnZero_WhenEndDateBeforeStartDate()
    {
        // Arrange
        var request = TestDataFactory.CreateValidPremiumRequest(
            startDate: DateTime.UtcNow.Date.AddDays(10),
            endDate: DateTime.UtcNow.Date.AddDays(1),
            type: CoverType.PassengerShip);

        // Act
        var response = await Client.PostAsJsonAsync(
            TestConstants.Routes.ComputePremium,
            request,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var premium = await response.ReadAsAsync<ComputePremiumResponse>();
        Assert.Equal(0m, premium.Amount);
    }
}
