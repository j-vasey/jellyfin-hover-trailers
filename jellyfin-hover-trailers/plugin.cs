using Jellyfin.Plugin.HoverTrailers.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Jellyfin.Plugin.HoverTrailers
{
    // 1. Add IHasWebPages to ensure GetPages() is recognized by the server
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public override string Name => "Jellyfin Hover Trailers";

        // Use your specific GUID from the manifest
        public override Guid Id => Guid.Parse("FCBEE76B-7170-410F-9029-B3B2E5F5EDA4");

        public static Plugin? Instance { get; private set; } = null;

        // 2. Make this PUBLIC so TransformationPatches.cs can access it
        internal IServerConfigurationManager ServerConfigurationManager { get; set; }

        public Plugin(
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer,
            IServerConfigurationManager serverConfigurationManager)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            ServerConfigurationManager = serverConfigurationManager;
        }

        // 3. Define the web resources served by the plugin
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
                    // Ensure the 'Web' folder and filename match your project structure exactly
                    EmbeddedResourcePath = GetType().Namespace + ".Web.hoverTrailers.js"
                }
            };
        }
    }
}