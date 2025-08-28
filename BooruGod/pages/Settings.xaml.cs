using Microsoft.Maui.Storage;
using BooruGod.Services;

namespace BooruGod.pages;

public partial class Settings : ContentPage
{
	private const string ApiKeyPreference = "Rule34ApiKey";
	private const string UserIdPreference = "Rule34UserId";

	public Settings()
	{
		InitializeComponent();
		LoadConfiguration();
		SetCurrentVersion();
	}

	private void LoadConfiguration()
	{
		var apiKey = Preferences.Get(ApiKeyPreference, string.Empty);
		var userId = Preferences.Get(UserIdPreference, string.Empty);

		ApiKeyEntry.Text = apiKey;
		UserIdEntry.Text = userId;
	}

	private async void OnSaveClicked(object? sender, EventArgs e)
	{
		var apiKey = ApiKeyEntry.Text?.Trim();
		var userId = UserIdEntry.Text?.Trim();

		if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(userId))
		{
			StatusLabel.Text = "Please enter both API key and User ID";
			StatusLabel.TextColor = Colors.Red;
			return;
		}

		if (!int.TryParse(userId, out _))
		{
			StatusLabel.Text = "User ID must be a valid number";
			StatusLabel.TextColor = Colors.Red;
			return;
		}

		Preferences.Set(ApiKeyPreference, apiKey);
		Preferences.Set(UserIdPreference, userId);

		StatusLabel.Text = "Configuration saved successfully!";
		StatusLabel.TextColor = Colors.Green;

		await Task.Delay(2000);
		StatusLabel.Text = string.Empty;
	}

	private async void OnApiUrlTapped(object? sender, EventArgs e)
	{
		try
		{
			var url = "https://rule34.xxx/index.php?page=account&s=options";
			await Launcher.OpenAsync(new Uri(url));
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Could not open URL: {ex.Message}", "OK");
		}
	}

	private void SetCurrentVersion()
	{
		try
		{
			var version = VersionTracking.CurrentVersion;
			CurrentVersionLabel.Text = $"Current Version: v{version}";
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[Settings] Error setting version: {ex.Message}");
			CurrentVersionLabel.Text = "Current Version: v1.0";
		}
	}

	private async void OnCheckUpdateClicked(object? sender, EventArgs e)
	{
		try
		{
			CheckUpdateButton.IsEnabled = false;
			CheckUpdateButton.Text = "Checking...";
			UpdateStatusLabel.Text = "Checking for updates...";
			UpdateStatusLabel.TextColor = Colors.Blue;

			var updateService = new UpdateService();
			var updateInfo = await updateService.CheckForUpdates();

			if (updateInfo != null)
			{
				var isMandatory = await updateService.IsUpdateMandatory(updateInfo);
				UpdateStatusLabel.Text = $"Update available: v{updateInfo.Version}";
				UpdateStatusLabel.TextColor = Colors.Green;

				// Show update dialog
				await Navigation.PushAsync(new UpdateDialog(updateInfo, isMandatory));
			}
			else
			{
				UpdateStatusLabel.Text = "No updates available";
				UpdateStatusLabel.TextColor = Colors.Green;
			}
		}
		catch (Exception ex)
		{
			UpdateStatusLabel.Text = $"Error checking updates: {ex.Message}";
			UpdateStatusLabel.TextColor = Colors.Red;
		}
		finally
		{
			CheckUpdateButton.IsEnabled = true;
			CheckUpdateButton.Text = "Check for Updates";
		}
	}
}
