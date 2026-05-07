using Claims.Application.Abstractions.Services;
using Claims.IntegrationTests.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Claims.IntegrationTests.Infrastructure.Hosting;

public sealed class ClaimsWebApplicationFactory : WebApplicationFactory<Program>
{
    public ClaimsMongoTestStore DataStore => Services.GetRequiredService<ClaimsMongoTestStore>();
    public ValidationSettings Validation { get; }

    public ClaimsWebApplicationFactory()
    {
        Validation = new ValidationSettings(this);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder
                .AddJsonFile(GetIntegrationSettingsPath(), optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            // Keep app startup deterministic for integration tests by disabling background sender.
            services.RemoveAll<IHostedService>();
            services.RemoveAll<IServiceBusService>();
            services.AddSingleton<IServiceBusService, MockServiceBusService>();
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await DataStore.ResetAsync();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<ClaimsMongoTestStore>();
        });

        return base.CreateHost(builder);
    }

    private static string GetIntegrationSettingsPath()
        => Path.Combine(AppContext.BaseDirectory, "appsettings.IntegrationTests.json");

    public sealed class ValidationSettings(
        ClaimsWebApplicationFactory factory)
    {
        public decimal MaxClaimDamageCost =>
            factory.Services.GetRequiredService<IConfiguration>()
                .GetValue<decimal>("ValidationRules:Claims:MaxDamageCost");

        public int MaxCoverInsurancePeriodYears =>
            factory.Services.GetRequiredService<IConfiguration>()
                .GetValue<int>("ValidationRules:Covers:MaxInsurancePeriodYears");
    }
}
