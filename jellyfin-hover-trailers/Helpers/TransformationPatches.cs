using System.Text.RegularExpressions;
using MediaBrowser.Common.Net;

namespace Jellyfin.Plugin.HoverTrailers.Helpers
{
    public static class TransformationPatches
    {
        public static string IndexHtml(object payload)
        {
            // Use reflection to get 'contents' safely from the FileTransformation plugin
            var contents = payload.GetType().GetProperty("contents")?.GetValue(payload) as string;

            if (string.IsNullOrEmpty(contents)) return string.Empty;

            // 1. Handle BaseUrl (Root Path) automatically
            // This ensures it works if the user uses a reverse proxy like /jellyfin
            string rootPath = "";
            try
            {
                var networkConfig = Plugin.Instance?.ServerConfigurationManager?.GetNetworkConfiguration();
                if (networkConfig != null && !string.IsNullOrWhiteSpace(networkConfig.BaseUrl))
                {
                    rootPath = $"/{networkConfig.BaseUrl.TrimStart('/').Trim()}";
                }
            }
            catch { /* Fallback to empty if config isn't ready */ }

            // 2. Define your script element (Using the Name from GetPages)
            string scriptUrl = $"{rootPath}/Plugins/FCBEE76B-7170-410F-9029-B3B2E5F5EDA4/Configuration?name=hoverTrailers";
            string scriptElement = $"<script src=\"{scriptUrl}\" defer></script>";

            // 3. Simple Injection before closing body tag
            return Regex.Replace(contents, "(</body>)", $"{scriptElement}$1");
        }
    }
}