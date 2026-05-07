namespace Claims.IntegrationTests.Infrastructure.Hosting;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected ClaimsWebApplicationFactory Factory { get; } = new();

    protected HttpClient Client { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        Client = Factory.CreateClient();
        await Factory.ResetDatabaseAsync();
    }

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        Factory.Dispose();

        return ValueTask.CompletedTask;
    }
}
