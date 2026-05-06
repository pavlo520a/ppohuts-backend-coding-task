using Claims.Data.Auditing;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddAuditing();
    })
    .Build();

host.Run();
