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
}
