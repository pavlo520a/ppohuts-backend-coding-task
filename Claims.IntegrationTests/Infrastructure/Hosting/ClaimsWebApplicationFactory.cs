using Claims.Application.Abstractions.Services;
using Claims.IntegrationTests.Infrastructure.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Claims.IntegrationTests.Infrastructure.Hosting;

public sealed class ClaimsWebApplicationFactory : WebApplicationFactory<Program>
{
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

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped<ClaimsDataStore>();
        });

        return base.CreateHost(builder);
    }

    private static string GetIntegrationSettingsPath()
        => Path.Combine(AppContext.BaseDirectory, "appsettings.IntegrationTests.json");
}
