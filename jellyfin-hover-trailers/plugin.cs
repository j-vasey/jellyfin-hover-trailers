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
        public override string Name => "Hover Trailers";
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
            return new[]
            {
                // Settings Page
                new PluginPageInfo
                {
                    Name = "HoverTrailers",
                    EmbeddedResourcePath = "Jellyfin.Plugin.HoverTrailers.Configuration.configPage.html"
                },
                // The script itself
                new PluginPageInfo
                {
                    Name = "hoverTrailersScript",
                    EmbeddedResourcePath = "Jellyfin.Plugin.HoverTrailers.Web.hoverTrailers.js",
                    EnableInMainMenu = false // This prevents it from showing up as a button
                }
            };
        }
    };
}
