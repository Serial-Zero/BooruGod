using System.Text.Json;

namespace BooruGod.Services
{
    public class UpdateInfo
    {
        public string Version { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string ReleaseNotes { get; set; } = string.Empty;
        public bool Mandatory { get; set; }
        public string MinVersion { get; set; } = string.Empty;
    }

    public class UpdateService
    {
        // Update this URL to point to your GitHub repository
        private const string VERSION_URL = "https://raw.githubusercontent.com/Serial-Zero/BooruGod/main/version.json";
        private const string GITHUB_RELEASES_URL = "https://github.com/Serial-Zero/BooruGod/releases";
        
        public async Task<UpdateInfo?> CheckForUpdates()
        {
            try
            {
                // Get current app version
                var currentVersion = Version.Parse(VersionTracking.CurrentVersion);
                
                // Download version.json from GitHub
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                var json = await client.GetStringAsync(VERSION_URL);
                
                var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(json);
                if (updateInfo == null) return null;
                
                // Parse the new version
                if (!Version.TryParse(updateInfo.Version, out var newVersion))
                    return null;
                
                // Check if update is available
                if (newVersion > currentVersion)
                {
                    return updateInfo;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateService] Error checking for updates: {ex.Message}");
                return null;
            }
        }
        
        public async Task<bool> IsUpdateMandatory(UpdateInfo updateInfo)
        {
            try
            {
                var currentVersion = Version.Parse(VersionTracking.CurrentVersion);
                var minVersion = Version.Parse(updateInfo.MinVersion);
                
                return currentVersion < minVersion || updateInfo.Mandatory;
            }
            catch
            {
                return updateInfo.Mandatory;
            }
        }
        
        public async Task OpenDownloadPage(string downloadUrl)
        {
            try
            {
                await Launcher.OpenAsync(new Uri(downloadUrl));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateService] Error opening download URL: {ex.Message}");
                // Fallback to GitHub releases page
                await Launcher.OpenAsync(new Uri(GITHUB_RELEASES_URL));
            }
        }
    }
}
