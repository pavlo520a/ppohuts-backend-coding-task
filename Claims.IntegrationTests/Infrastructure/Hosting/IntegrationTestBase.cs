using Claims.IntegrationTests.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.IntegrationTests.Infrastructure.Hosting;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private AsyncServiceScope dataStoreScope;

    protected ClaimsWebApplicationFactory Factory { get; } = new();
    protected ClaimsDataStore DataStore { get; private set; } = null!;
    protected IConfiguration Configuration { get; private set; } = null!;

    protected HttpClient Client { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        Client = Factory.CreateClient();
        dataStoreScope = Factory.Services.CreateAsyncScope();
        DataStore = dataStoreScope.ServiceProvider.GetRequiredService<ClaimsDataStore>();
        Configuration = dataStoreScope.ServiceProvider.GetRequiredService<IConfiguration>();

        await DataStore.ResetAsync();
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await dataStoreScope.DisposeAsync();
        Factory.Dispose();
    }
}
