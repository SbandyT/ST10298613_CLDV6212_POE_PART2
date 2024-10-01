using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

    })
    .ConfigureWebJobs(b =>
    {
        
        b.AddHttp();
        b.AddAzureStorageBlobs(); 
        b.AddAzureStorageQueues(); 
        
    })
    .Build();

host.Run();
