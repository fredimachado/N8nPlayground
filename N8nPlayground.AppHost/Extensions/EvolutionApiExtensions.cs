using Aspire.Hosting;

namespace N8nPlayground.AppHost.Extensions;

public static class EvolutionApiExtensions
{
    /// <summary>
    /// Adds an EvolutionAPI container to the distributed application.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="port">An optional fixed port to bind to the EvolutionAPI container. This will be provided randomly by Aspire if not set.</param>
    /// <returns></returns>
    public static IResourceBuilder<ContainerResource> AddEvolutionApi(
        this IDistributedApplicationBuilder builder,
        string name = "evolution-api",
        int? port = 38080)
    {
        return builder
            .AddContainer(name, "atendai/evolution-api", "v2.2.3")
            .WithEnvironment("SERVER_TYPE", "http")
            .WithEnvironment("SERVER_PORT", "8080")
            .WithEnvironment("SERVER_URL", "http://localhost:8080")
            .WithEnvironment("CORS_ORIGIN", "*")
            .WithEnvironment("CORS_METHODS", "GET,POST,PUT,DELETE")
            .WithEnvironment("CORS_CREDENTIALS", "true")
            .WithEnvironment("LOG_LEVEL", "ERROR,WARN,DEBUG,INFO,LOG,VERBOSE,DARK,WEBHOOKS,WEBSOCKET")
            .WithEnvironment("LOG_COLOR", "true")
            .WithEnvironment("LOG_BAILEYS", "error")
            .WithEnvironment("EVENT_EMITTER_MAX_LISTENERS", "50")
            .WithEnvironment("DEL_INSTANCE", "false")
            .WithEnvironment("DATABASE_PROVIDER", "postgresql")
            .WithEnvironment("DATABASE_CONNECTION_CLIENT_NAME", "evolution_exchange")
            .WithEnvironment("DATABASE_SAVE_DATA_INSTANCE", "true")
            .WithEnvironment("DATABASE_SAVE_DATA_NEW_MESSAGE", "true")
            .WithEnvironment("DATABASE_SAVE_MESSAGE_UPDATE", "true")
            .WithEnvironment("DATABASE_SAVE_DATA_CONTACTS", "true")
            .WithEnvironment("DATABASE_SAVE_DATA_CHATS", "true")
            .WithEnvironment("DATABASE_SAVE_DATA_LABELS", "true")
            .WithEnvironment("DATABASE_SAVE_DATA_HISTORIC", "true")
            .WithEnvironment("DATABASE_SAVE_IS_ON_WHATSAPP", "true")
            .WithEnvironment("DATABASE_SAVE_IS_ON_WHATSAPP_DAYS", "7")
            .WithEnvironment("DATABASE_DELETE_MESSAGE", "true")
            .WithEnvironment("RABBITMQ_ENABLED", "false")
            .WithEnvironment("SQS_ENABLED", "false")
            .WithEnvironment("WEBSOCKET_ENABLED", "false")
            .WithEnvironment("WEBSOCKET_GLOBAL_EVENTS", "false")
            .WithEnvironment("PUSHER_ENABLED", "false")
            .WithEnvironment("PUSHER_GLOBAL_ENABLED", "false")
            .WithEnvironment("WA_BUSINESS_TOKEN_WEBHOOK", "evolution")
            .WithEnvironment("WA_BUSINESS_URL", "https://graph.facebook.com")
            .WithEnvironment("WA_BUSINESS_VERSION", "v20.0")
            .WithEnvironment("WA_BUSINESS_LANGUAGE", "en_US")
            .WithEnvironment("WEBHOOK_GLOBAL_ENABLED", "false")
            .WithEnvironment("CONFIG_SESSION_PHONE_CLIENT", "Evolution API")
            .WithEnvironment("CONFIG_SESSION_PHONE_NAME", "Chrome")
            .WithEnvironment("CONFIG_SESSION_PHONE_VERSION", "2.3000.0")
            .WithEnvironment("QRCODE_LIMIT", "30")
            .WithEnvironment("QRCODE_COLOR", "#175197")
            .WithEnvironment("TYPEBOT_ENABLED", "false")
            .WithEnvironment("TYPEBOT_API_VERSION", "latest")
            .WithEnvironment("CHATWOOT_ENABLED", "false")
            .WithEnvironment("OPENAI_ENABLED", "false")
            .WithEnvironment("DIFY_ENABLED", "false")
            .WithEnvironment("CACHE_REDIS_ENABLED", "false")
            .WithEnvironment("CACHE_REDIS_TTL", "604800")
            .WithEnvironment("CACHE_REDIS_PREFIX_KEY", "evolution")
            .WithEnvironment("CACHE_REDIS_SAVE_INSTANCES", "false")
            .WithEnvironment("CACHE_LOCAL_ENABLED", "false")
            .WithEnvironment("S3_ENABLED", "false")
            .WithEnvironment("AUTHENTICATION_API_KEY", "123683C4C922415CAAFCCE10F7D57E11")
            .WithEnvironment("AUTHENTICATION_EXPOSE_IN_FETCH_INSTANCES", "true")
            .WithEnvironment("LANGUAGE", "en")
            .WithBindMount("../n8n/data", "/home/node/.n8n")
            .WithBindMount("../n8n/credentials", "/shared-data/credentials")
            .WithHttpEndpoint(port, targetPort: 8080);
    }
}
