using System.Globalization;

namespace N8nPlayground.AppHost.Extensions;

public static class N8nExtensions
{
    /// <summary>
    /// Adds an N8N container to the distributed application.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="port">An optional fixed port to bind to the N8N container. This will be provided randomly by Aspire if not set.</param>
    /// <param name="timeZone">An optional timezone in IANA format, like 'Australia/Sydney'. If not set, we try to detect the timezone of the OS.</param>
    /// <returns></returns>
    public static IResourceBuilder<ContainerResource> AddN8n(
        this IDistributedApplicationBuilder builder,
        string name = "n8n",
        int? port = 5678,
        string? timeZone = null)
    {
        if (timeZone is null)
        {
            TimeZoneInfo.TryConvertWindowsIdToIanaId(
                TimeZoneInfo.Local.Id,
                RegionInfo.CurrentRegion.TwoLetterISORegionName,
                out timeZone);
        }
        
        return builder
            .AddContainer(name, "n8nio/n8n", "1.89.2")
            .WithEnvironment("GENERIC_TIMEZONE", timeZone)
            .WithEnvironment("TZ", timeZone)
            .WithEnvironment("N8N_RUNNERS_ENABLED", "true")
            .WithEnvironment("N8N_DIAGNOSTICS_ENABLED", "false")
            .WithEnvironment("N8N_PERSONALIZATION_ENABLED", "false")
            .WithBindMount("../n8n/data", "/home/node/.n8n")
            .WithHttpEndpoint(port, targetPort: 5678);
    }
}
