using System.Reflection;
using System.Runtime.Loader;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.HoverTrailers.Helpers
{
    public class StartupService : IScheduledTask
    {
        private readonly ILogger<StartupService> _logger;

        public StartupService(ILogger<StartupService> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering Hover Trailers with File Transformation...");

            // Prepare the payload specifically for index.html
            var payload = new
            {
                id = "FCBEE76B-7170-410F-9029-B3B2E5F5EDA4",
                fileNamePattern = "index.html",
                callbackAssembly = GetType().Assembly.FullName,
                callbackClass = typeof(TransformationPatches).FullName,
                callbackMethod = nameof(TransformationPatches.IndexHtml)
            };

            // Locate the File Transformation plugin via Reflection
            Assembly? fileTransformationAssembly = AssemblyLoadContext.All
                .SelectMany(x => x.Assemblies)
                .FirstOrDefault(x => x.FullName?.Contains(".FileTransformation") ?? false);

            if (fileTransformationAssembly != null)
            {
                // Note: The other plugin uses "PluginInterface", check your version of FileTransformation
                Type? pluginInterfaceType = fileTransformationAssembly.GetType("Jellyfin.Plugin.FileTransformation.PluginInterface")
                                         ?? fileTransformationAssembly.GetType("Jellyfin.Plugin.FileTransformation.Plugin");

                if (pluginInterfaceType != null)
                {
                    pluginInterfaceType.GetMethod("RegisterTransformation")?.Invoke(null, new object?[] { payload });
                    _logger.LogInformation("Successfully registered Hover Trailers transformation.");
                }
            }
            else
            {
                _logger.LogWarning("File Transformation plugin not found. Automatic injection will not work.");
            }

            return Task.CompletedTask;
        }

        // This ensures the task runs automatically as soon as Jellyfin starts
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers() => StartupServiceHelper.GetDefaultTriggers();
        public string Name => "Hover Trailers Startup";
        public string Key => "Jellyfin.Plugin.HoverTrailers.Startup";
        public string Description => "Injects the hover trailers script into the web interface.";
        public string Category => "Maintenance";
    }
}