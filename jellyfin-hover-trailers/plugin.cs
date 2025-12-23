using System;
using System.Collections.Generic;
using Jellyfin.Plugin.HoverTrailers.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.HoverTrailers
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public override string Name => "Jellyfin Hover Trailers";
        public override Guid Id => Guid.Parse("FCBEE76B-7170-410F-9029-B3B2E5F5EDA4"); // Use Visual Studio > Tools > Create GUID

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public static Plugin Instance { get; private set; }

        // This links your HTML file to the Jellyfin Dashboard
        public IEnumerable<PluginPageInfo> GetPages()
        {
            var assemblyName = GetType().Assembly.GetName().Name;

            return new[]
            {
        // Settings page
        new PluginPageInfo
        {
            Name = "HoverTrailers",
            EmbeddedResourcePath = $"{assemblyName}.Configuration.configPage.html"
        },
        // The hover logic script
        new PluginPageInfo
        {
            Name = "hoverTrailersScript", // Internal name Jellyfin uses to serve the file
            EmbeddedResourcePath = $"{assemblyName}.Web.hoverTrailers.js",
            EnableInMainMenu = false
        }
    };
        }
    };
}
