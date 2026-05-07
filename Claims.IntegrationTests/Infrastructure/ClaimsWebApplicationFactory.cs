using Claims.Application.Abstractions.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Claims.IntegrationTests.Infrastructure;

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

    public async Task ResetDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var stateManager = scope.ServiceProvider.GetRequiredService<DatabaseStateManager>();
        await stateManager.ResetAsync();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped<DatabaseStateManager>();
        });

        return base.CreateHost(builder);
    }

    private static string GetIntegrationSettingsPath()
    {
        var outputPath = Path.Combine(AppContext.BaseDirectory, "appsettings.IntegrationTests.json");
        
        if (File.Exists(outputPath))
        {
            return outputPath;
        }

        var projectPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "appsettings.IntegrationTests.json"));

        return projectPath;
    }

}
