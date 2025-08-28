using BooruGod.Services;
using Microsoft.Maui.ApplicationModel;

namespace BooruGod
{
    public partial class MainPage : ContentPage
    {
        private bool isSidebarOpen = false;

        public MainPage()
        {
            InitializeComponent();
            SetVersionNumber();
            CheckForUpdates();
        }

        private async void OnMenuClicked(object? sender, EventArgs e)
        {
            if (!isSidebarOpen)
            {
                await OpenSidebar();
            }
            else
            {
                await CloseSidebar();
            }
        }

        private async void OnOverlayTapped(object? sender, EventArgs e)
        {
            await CloseSidebar();
        }

        private async void OnRule34Clicked(object? sender, EventArgs e)
        {
            await CloseSidebar();
            await Navigation.PushAsync(new pages.Rule34());
        }

        private async void OnSettingsClicked(object? sender, EventArgs e)
        {
            await CloseSidebar();
            await Navigation.PushAsync(new pages.Settings());
        }

        private async Task OpenSidebar()
        {
            isSidebarOpen = true;
            SidebarOverlay.IsVisible = true;
            
            await Task.WhenAll(
                SidebarOverlay.FadeTo(0.5, 250, Easing.CubicOut),
                Sidebar.TranslateTo(0, 0, 250, Easing.CubicOut)
            );
        }

        private async Task CloseSidebar()
        {
            isSidebarOpen = false;
            
            await Task.WhenAll(
                SidebarOverlay.FadeTo(0, 250, Easing.CubicIn),
                Sidebar.TranslateTo(-250, 0, 250, Easing.CubicIn)
            );
            
            SidebarOverlay.IsVisible = false;
        }

        private void SetVersionNumber()
        {
            try
            {
                var version = VersionTracking.CurrentVersion;
                VersionLabel.Text = $"v{version}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Error setting version number: {ex.Message}");
                VersionLabel.Text = "v1.0";
            }
        }

        private async void CheckForUpdates()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[MainPage] Starting update check...");
                var updateService = new UpdateService();
                var updateInfo = await updateService.CheckForUpdates();
                
                System.Diagnostics.Debug.WriteLine($"[MainPage] Update check result: {(updateInfo != null ? $"Update available: {updateInfo.Version}" : "No update available")}");
                
                if (updateInfo != null)
                {
                    var isMandatory = await updateService.IsUpdateMandatory(updateInfo);
                    System.Diagnostics.Debug.WriteLine($"[MainPage] Update is mandatory: {isMandatory}");
                    
                    if (isMandatory)
                    {
                        // For mandatory updates, try automatic installation
                        await InstallUpdateAutomatically(updateInfo);
                    }
                    else
                    {
                        // For optional updates, show dialog
                        await Navigation.PushAsync(new pages.UpdateDialog(updateInfo, isMandatory));
                    }
                }
            }
            catch (Exception ex)
            {
                // Silently handle update check errors
                System.Diagnostics.Debug.WriteLine($"[MainPage] Update check failed: {ex.Message}");
            }
        }

        private async Task InstallUpdateAutomatically(UpdateInfo updateInfo)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[MainPage] Attempting automatic installation...");
                
                // Download the APK to device storage
                using var client = new HttpClient();
                var apkBytes = await client.GetByteArrayAsync(updateInfo.DownloadUrl);
                
                // Save to downloads folder
                var downloadsPath = Path.Combine(FileSystem.AppDataDirectory, "Downloads");
                Directory.CreateDirectory(downloadsPath);
                var apkPath = Path.Combine(downloadsPath, "BooruGod_Update.apk");
                await File.WriteAllBytesAsync(apkPath, apkBytes);
                
                System.Diagnostics.Debug.WriteLine($"[MainPage] APK downloaded to: {apkPath}");
                
                // Open the file with the system's package installer
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(apkPath)
                });
                
                System.Diagnostics.Debug.WriteLine("[MainPage] Package installer launched");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPage] Automatic installation failed: {ex.Message}");
                
                // Fallback to manual installation dialog
                await Navigation.PushAsync(new pages.UpdateDialog(updateInfo, true));
            }
        }
    }
}
