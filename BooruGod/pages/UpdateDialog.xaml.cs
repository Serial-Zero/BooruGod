using BooruGod.Services;

namespace BooruGod.pages;

public partial class UpdateDialog : ContentPage
{
    private readonly UpdateInfo _updateInfo;
    private readonly UpdateService _updateService;
    private readonly bool _isMandatory;

    public UpdateDialog(UpdateInfo updateInfo, bool isMandatory = false)
    {
        InitializeComponent();
        _updateInfo = updateInfo;
        _isMandatory = isMandatory;
        _updateService = new UpdateService();
        
        LoadUpdateInfo();
    }

    private void LoadUpdateInfo()
    {
        VersionLabel.Text = $"Version {_updateInfo.Version} Available";
        ReleaseNotesLabel.Text = _updateInfo.ReleaseNotes;
        
        if (_isMandatory)
        {
            MandatoryNotice.IsVisible = true;
            LaterButton.IsVisible = false;
        }
    }

    private async void OnDownloadClicked(object sender, EventArgs e)
    {
        try
        {
            DownloadButton.IsEnabled = false;
            DownloadButton.Text = "📥 Downloading...";
            
            // Show progress dialog
            var progressDialog = await DisplayAlert(
                "Downloading Update", 
                "Downloading the update directly to your device...\n\nThis may take a few moments.", 
                "OK", 
                "Cancel"
            );
            
            if (!progressDialog)
            {
                DownloadButton.IsEnabled = true;
                DownloadButton.Text = "📥 Download Update";
                return;
            }
            
            // Try direct download first
            var downloadSuccess = await _updateService.DownloadUpdateToDevice(_updateInfo.DownloadUrl);
            
            if (downloadSuccess)
            {
                await DisplayAlert(
                    "Download Complete", 
                    "The update has been downloaded and the package installer should open automatically.\n\nIf it doesn't open, check your Downloads folder.", 
                    "OK"
                );
                
                // Close the dialog
                await Navigation.PopAsync();
            }
            else
            {
                // Fallback to browser download
                await DisplayAlert(
                    "Direct Download Failed", 
                    "Direct download failed. Opening browser download instead.", 
                    "OK"
                );
                
                await _updateService.OpenDownloadPage(_updateInfo.DownloadUrl);
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to download update: {ex.Message}", "OK");
            DownloadButton.IsEnabled = true;
            DownloadButton.Text = "📥 Download Update";
        }
    }

    private async void OnLaterClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
