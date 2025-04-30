using N8nPlayground.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgresPassword = builder.AddParameter("postgres-password", secret: true);
var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume("n8n-playground")
    .WithPgAdmin(resource =>
    {
        resource.WithUrlForEndpoint("http", u => u.DisplayText = "PG Admin");
    }); ;

var ollama = builder.AddOllama("ollama", 11434)
    .WithDataVolume("ollama")
    .WithGPUSupport();

//ollama.AddModel("gemma2:2b");
ollama.AddModel("llama3.2");

var n8n = builder.AddN8n()
    .WithEnvironment("OLLAMA_HOST", "ollama:11434")
    .WaitFor(ollama);

var evolutionDb = postgres.AddDatabase("evolution");
var evolutionApi = builder.AddEvolutionApi()
    .WithExternalHttpEndpoints()
    .WithEnvironment("DATABASE_CONNECTION_URI", $"postgresql://postgres:{postgresPassword.Resource.Value}@postgres:5432/{evolutionDb.Resource.DatabaseName}?sslmode=disable")
    .WaitFor(postgres);

var useNgrok = false;
if (useNgrok)
{
    // To safely store your NGrok auth token, use the following command inside  the N8nPlayground.AppHost folder:
    // dotnet user-secrets set "Parameters:ngrok-auth-token" "YOUR_NGROK_AUTH_TOKEN"
    var authToken = builder
        .AddParameter("ngrok-auth-token", secret: true);

    builder.AddNgrok("ngrok", endpointPort: 59600) // omit endpointPort to use random port
        .WithAuthToken(authToken)
        .WithTunnelEndpoint(evolutionApi, "http")
        .WithTunnelEndpoint(n8n, "http");
}

//var apiService = builder.AddProject<Projects.N8nPlayground_ApiService>("apiservice")
//    .WithHttpsHealthCheck("/health");

//builder.AddProject<Projects.N8nPlayground_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpsHealthCheck("/health")
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

builder.Build().Run();
