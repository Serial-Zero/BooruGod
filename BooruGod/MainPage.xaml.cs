using BooruGod.Services;

namespace BooruGod
{
    public partial class MainPage : ContentPage
    {
        private bool isSidebarOpen = false;

        public MainPage()
        {
            InitializeComponent();
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

        private async void CheckForUpdates()
        {
            try
            {
                var updateService = new UpdateService();
                var updateInfo = await updateService.CheckForUpdates();
                
                if (updateInfo != null)
                {
                    var isMandatory = await updateService.IsUpdateMandatory(updateInfo);
                    await Navigation.PushAsync(new pages.UpdateDialog(updateInfo, isMandatory));
                }
            }
            catch (Exception ex)
            {
                // Silently handle update check errors
                System.Diagnostics.Debug.WriteLine($"[MainPage] Update check failed: {ex.Message}");
            }
        }
    }
}
