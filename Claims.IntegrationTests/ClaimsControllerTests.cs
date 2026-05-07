using Claims.Application.Abstractions;
using Claims.Application.Commands.Claims;
using Claims.Domain.Enums;
using Claims.Domain.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Claims.IntegrationTests
{
    public class ClaimsControllerTests
    {
        [Fact]
        public async Task Get_Claims()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((_, configBuilder) =>
                    {
                        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["ServiceBus:ConnectionString"] = "Endpoint=sb://localhost/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=abc=",
                            ["ServiceBus:QueueName"] = "claims-audit-outbox"
                        });
                    });

                    builder.ConfigureServices(services =>
                    {
                        services.AddScoped<IUseCase<GetClaimsCommand, IReadOnlyList<Claim>>, StubGetClaimsUseCase>();
                    });
                });

            var client = application.CreateClient();

            var response = await client.GetAsync("/Claims", TestContext.Current.CancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            Assert.Contains("claim-1", content);
        }

    }

    internal sealed class StubGetClaimsUseCase : IUseCase<GetClaimsCommand, IReadOnlyList<Claim>>
    {
        public Task<IReadOnlyList<Claim>> ExecuteAsync(GetClaimsCommand command, CancellationToken cancellationToken)
        {
            IReadOnlyList<Claim> claims =
            [
                new Claim
                {
                    Id = "claim-1",
                    CoverId = "cover-1",
                    Created = DateTime.UtcNow,
                    Name = "Fire damage",
                    Type = ClaimType.Fire,
                    DamageCost = 1000m
                }
            ];

            return Task.FromResult(claims);
        }
    }
}
