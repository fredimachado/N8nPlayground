using N8nPlayground.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama", 11434)
    .WithDataVolume("ollama")
    .WithGPUSupport();

//ollama.AddModel("gemma2:2b");
ollama.AddModel("llama3.2");

builder.AddN8n()
    .WithEnvironment("OLLAMA_HOST", "ollama:11434")
    .WaitFor(ollama);

//var cache = builder.AddRedis("cache");

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
