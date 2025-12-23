using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.HoverTrailers.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string TmdbApiKey { get; set; } = string.Empty;
        
        // You can add more settings here later, like "Mute by default"
        public bool MuteTrailers { get; set; } = true;
    }
}