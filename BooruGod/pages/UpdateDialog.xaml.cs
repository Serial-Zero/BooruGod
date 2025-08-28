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
            DownloadButton.Text = "Opening...";
            
            await _updateService.OpenDownloadPage(_updateInfo.DownloadUrl);
            
            // Close the dialog
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to open download page: {ex.Message}", "OK");
            DownloadButton.IsEnabled = true;
            DownloadButton.Text = "📥 Download Update";
        }
    }

    private async void OnLaterClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
