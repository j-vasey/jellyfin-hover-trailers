using Jellyfin.Plugin.HoverTrailers.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Jellyfin.Plugin.HoverTrailers
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public override string Name => "Jellyfin Hover Trailers";
        public override Guid Id => Guid.Parse("FCBEE76B-7170-410F-9029-B3B2E5F5EDA4");

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            RegisterWithFileTransformation();
        }

        public static Plugin? Instance { get; private set; }

        private void RegisterWithFileTransformation()
        {
            // Prepare payload for File Transformation
            var payload = new
            {
                id = Id.ToString(),
                fileNamePattern = "index.html", // Target the main page
                callbackAssembly = GetType().Assembly.FullName,
                callbackClass = GetType().FullName,
                callbackMethod = nameof(InjectScript)
            };

            // Locate the File Transformation assembly via Reflection
            Assembly? fileTransformationAssembly = AssemblyLoadContext.All
                .SelectMany(x => x.Assemblies)
                .FirstOrDefault(x => x.FullName?.Contains(".FileTransformation") ?? false);

            if (fileTransformationAssembly != null)
            {
                Type? pluginInterfaceType = fileTransformationAssembly.GetType("Jellyfin.Plugin.FileTransformation.Plugin");
                pluginInterfaceType?.GetMethod("RegisterTransformation")?.Invoke(null, new object?[] { payload });
            }
        }

        // This method is called by the File Transformation plugin
        public static string InjectScript(object input)
        {
            // Access the 'contents' property from the anonymous object passed in
            var contents = input.GetType().GetProperty("contents")?.GetValue(input) as string ?? string.Empty;

            string scriptTag = $"<script src=\"/Plugins/{Instance?.Id}/Configuration?name=hoverTrailers\"></script>";

            // Inject the script before the closing </head> tag
            return contents.Replace("</head>", $"{scriptTag}\n</head>");
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "HoverTrailersConfig",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.configPage.html"
                },
                new PluginPageInfo
                {
                    Name = "hoverTrailers",
                    EmbeddedResourcePath = GetType().Namespace + ".Web.hoverTrailers.js"
                }
            };
        }
    }
}