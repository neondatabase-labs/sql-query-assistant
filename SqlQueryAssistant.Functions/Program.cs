using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<EmbeddingService>();
        services.AddSingleton<ChatCompletionService>();
        services.AddSingleton<SqlExecutorService>();
        services.AddSingleton<SchemaRetrievalService>();
        services.AddSingleton<VectorStorageService>();
        services.AddSingleton<SchemaService>();
    })
    .Build();

host.Run();
