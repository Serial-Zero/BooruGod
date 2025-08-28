using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Maui.ApplicationModel;

namespace BooruGod.Services
{
    public class UpdateInfo
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;
        
        [JsonPropertyName("downloadUrl")]
        public string DownloadUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("releaseNotes")]
        public string ReleaseNotes { get; set; } = string.Empty;
        
        [JsonPropertyName("mandatory")]
        public bool Mandatory { get; set; }
        
        [JsonPropertyName("minVersion")]
        public string MinVersion { get; set; } = string.Empty;
    }

    public class UpdateService
    {
        // Update this URL to point to your GitHub repository
        private const string VERSION_URL = "https://raw.githubusercontent.com/Serial-Zero/BooruGod/main/version.json";
        private const string GITHUB_RELEASES_URL = "https://github.com/Serial-Zero/BooruGod/releases";
        
        // Test URL to verify network connectivity
        private const string TEST_URL = "https://httpbin.org/get";
        
        public async Task<(UpdateInfo? updateInfo, string debugInfo)> CheckForUpdatesWithDebug()
        {
            var debugInfo = new List<string>();
            
            try
            {
                debugInfo.Add("Starting update check...");
                
                // Check and request internet permissions first
                var permissionStatus = await CheckAndRequestInternetPermission();
                if (!permissionStatus)
                {
                    debugInfo.Add("❌ Internet permission denied or not granted");
                    return (null, string.Join("\n", debugInfo));
                }
                debugInfo.Add("✅ Internet permission check passed");
                
                // Get current app version
                var currentVersion = Version.Parse(VersionTracking.CurrentVersion);
                debugInfo.Add($"📱 Current app version: {currentVersion}");
                debugInfo.Add($"📱 VersionTracking.CurrentVersion raw: {VersionTracking.CurrentVersion}");
                
                // Download version.json from GitHub
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                debugInfo.Add($"🌐 Fetching from URL: {VERSION_URL}");
                
                // Test network connectivity first
                try
                {
                    debugInfo.Add("🔍 Testing basic network connectivity...");
                    var testResponse = await client.GetAsync(TEST_URL);
                    debugInfo.Add($"🔍 Basic network test - Status Code: {testResponse.StatusCode}");
                    
                    if (!testResponse.IsSuccessStatusCode)
                    {
                        debugInfo.Add($"❌ Basic network test failed with status: {testResponse.StatusCode}");
                        return (null, string.Join("\n", debugInfo));
                    }
                    
                    debugInfo.Add("✅ Basic network connectivity OK");
                    debugInfo.Add("🔍 Testing GitHub URL...");
                    var githubResponse = await client.GetAsync(VERSION_URL);
                    debugInfo.Add($"🔍 GitHub URL - Status Code: {githubResponse.StatusCode}");
                    debugInfo.Add($"🔍 GitHub URL - IsSuccessStatusCode: {githubResponse.IsSuccessStatusCode}");
                    
                    if (!githubResponse.IsSuccessStatusCode)
                    {
                        debugInfo.Add($"❌ GitHub URL failed with status: {githubResponse.StatusCode}");
                        return (null, string.Join("\n", debugInfo));
                    }
                    debugInfo.Add("✅ GitHub URL accessible");
                }
                catch (Exception networkEx)
                {
                    debugInfo.Add($"❌ Network error: {networkEx.Message}");
                    debugInfo.Add($"❌ Network error type: {networkEx.GetType().Name}");
                    return (null, string.Join("\n", debugInfo));
                }
                
                var json = await client.GetStringAsync(VERSION_URL);
                debugInfo.Add($"📄 Downloaded version.json: {json}");
                
                var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(json);
                if (updateInfo == null) 
                {
                    debugInfo.Add("❌ Failed to deserialize UpdateInfo");
                    return (null, string.Join("\n", debugInfo));
                }
                
                debugInfo.Add($"📄 Parsed UpdateInfo - Version: {updateInfo.Version}");
                debugInfo.Add($"📄 Version string length: {updateInfo.Version?.Length ?? 0}");
                debugInfo.Add($"📄 Version string is null: {updateInfo.Version == null}");
                debugInfo.Add($"📄 Version string is empty: {string.IsNullOrEmpty(updateInfo.Version)}");
                debugInfo.Add($"📄 Version string trimmed: '{updateInfo.Version?.Trim()}'");
                
                // Parse the new version
                if (!Version.TryParse(updateInfo.Version, out var newVersion))
                {
                    debugInfo.Add($"❌ Failed to parse version: {updateInfo.Version}");
                    debugInfo.Add($"❌ Version string type: {updateInfo.Version?.GetType().Name ?? "null"}");
                    return (null, string.Join("\n", debugInfo));
                }
                
                debugInfo.Add($"🆕 New version from GitHub: {newVersion}");
                debugInfo.Add($"🔍 Version comparison: {newVersion} > {currentVersion} = {newVersion > currentVersion}");
                
                // Check if update is available
                if (newVersion > currentVersion)
                {
                    debugInfo.Add("✅ Update is available!");
                    return (updateInfo, string.Join("\n", debugInfo));
                }
                else
                {
                    debugInfo.Add("ℹ️ No update available - current version is >= new version");
                }
                
                return (null, string.Join("\n", debugInfo));
            }
            catch (Exception ex)
            {
                debugInfo.Add($"❌ Error checking for updates: {ex.Message}");
                debugInfo.Add($"❌ Stack trace: {ex.StackTrace}");
                return (null, string.Join("\n", debugInfo));
            }
        }
        
        public async Task<UpdateInfo?> CheckForUpdates()
        {
            var (updateInfo, debugInfo) = await CheckForUpdatesWithDebug();
            System.Diagnostics.Debug.WriteLine($"[UpdateService] Debug Info:\n{debugInfo}");
            return updateInfo;
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
        
        public async Task<bool> DownloadUpdateToDevice(string downloadUrl)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateService] Starting direct download from: {downloadUrl}");
                
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(5); // 5 minute timeout for large file
                
                // Download the APK file
                var apkBytes = await client.GetByteArrayAsync(downloadUrl);
                System.Diagnostics.Debug.WriteLine($"[UpdateService] Downloaded {apkBytes.Length} bytes");
                
                // Get the Downloads folder path
                var downloadsPath = Path.Combine(FileSystem.AppDataDirectory, "Downloads");
                Directory.CreateDirectory(downloadsPath);
                
                // Create a unique filename with timestamp
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var apkPath = Path.Combine(downloadsPath, $"BooruGod_v2.0.1_{timestamp}.apk");
                
                // Save the APK file
                await File.WriteAllBytesAsync(apkPath, apkBytes);
                System.Diagnostics.Debug.WriteLine($"[UpdateService] APK saved to: {apkPath}");
                
                // Open the file with the system's package installer
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(apkPath)
                });
                
                System.Diagnostics.Debug.WriteLine("[UpdateService] Package installer launched successfully");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateService] Error downloading update: {ex.Message}");
                return false;
            }
        }
        
        public async Task OpenDownloadPage(string downloadUrl)
        {
            try
            {
                // Try direct download first
                var downloadSuccess = await DownloadUpdateToDevice(downloadUrl);
                if (!downloadSuccess)
                {
                    // Fallback to browser download
                    await Launcher.OpenAsync(new Uri(downloadUrl));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateService] Error opening download URL: {ex.Message}");
                // Fallback to GitHub releases page
                await Launcher.OpenAsync(new Uri(GITHUB_RELEASES_URL));
            }
        }
        
        private async Task<bool> CheckAndRequestInternetPermission()
        {
            try
            {
                // For Android, internet permission is typically granted at install time
                // We'll just return true and let the network request fail naturally if there's no internet
                System.Diagnostics.Debug.WriteLine("[UpdateService] Internet permission check - assuming granted (Android default)");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateService] Error checking internet permission: {ex.Message}");
                return true;
            }
        }
        
        // Simple test method to verify version.json accessibility
        public async Task<string> TestVersionJsonAccess()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                var json = await client.GetStringAsync(VERSION_URL);
                return $"Success: {json}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
