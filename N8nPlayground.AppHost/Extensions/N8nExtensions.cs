using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
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

        const string containerName = "n8n-playground";

        var containerBuilder = builder
            .AddContainer(name, "n8nio/n8n", "1.89.2")
            .WithContainerName(containerName)
            .WithEnvironment("GENERIC_TIMEZONE", timeZone)
            .WithEnvironment("TZ", timeZone)
            .WithEnvironment("N8N_RUNNERS_ENABLED", "true")
            .WithEnvironment("N8N_DIAGNOSTICS_ENABLED", "false")
            .WithEnvironment("N8N_PERSONALIZATION_ENABLED", "false")
            .WithEnvironment("N8N_DEFAULT_BINARY_DATA_MODE", "filesystem")
            .WithEnvironment("N8N_ENCRYPTION_KEY", "super-secret-key") // Changing this will invalidate all existing credentials
            .WithBindMount("../n8n/data", "/home/node/.n8n")
            .WithBindMount("../n8n/credentials", "/shared-data/credentials")
            .WithHttpEndpoint(port, targetPort: 5678);

        // Import existing credentials
        builder.Eventing.Subscribe<ResourceReadyEvent>(containerBuilder.Resource, (@event, cancellationToken) =>
        {
            var loggerService = @event.Services.GetRequiredService<ResourceLoggerService>();

            if (containerBuilder.Resource is not ContainerResource containerResource)
            {
                return Task.CompletedTask;
            }

            var logger = loggerService.GetLogger(containerBuilder.Resource);

            _ = Task.Run(async () =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = $"/c docker exec -d {containerName} sh -c \"sleep 3; n8n import:credentials --separate --input=/shared-data/credentials\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    }
                };

                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrWhiteSpace(args.Data))
                    {
                        logger.LogInformation("{Data}", args.Data);
                    }
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrWhiteSpace(args.Data))
                    {
                        logger.LogError("{Data}", args.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

                if (process.ExitCode != 0)
                {
                    logger.LogError("Command exited with {ExitCode}", process.ExitCode);
                }
            }, cancellationToken);

            return Task.CompletedTask;
        });

        return containerBuilder;
    }
}
