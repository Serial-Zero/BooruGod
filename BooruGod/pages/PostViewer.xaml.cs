using BooruGod.API_Logic.Rule34;
using BooruGod.Functions.MediaPlayer;

namespace BooruGod.pages;

public partial class PostViewer : ContentPage
{
    private readonly Rule34Post _post;
    private readonly Rule34Service _rule34Service;

    public PostViewer(Rule34Post post, Rule34Service rule34Service)
    {
        InitializeComponent();
        _post = post;
        _rule34Service = rule34Service;
        LoadPostDetails();
    }

    private void LoadPostDetails()
    {
        try
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"[PostViewer] FileExt: {_post.FileExt}");
            System.Diagnostics.Debug.WriteLine($"[PostViewer] IsVideo: {_post.IsVideo}");
            System.Diagnostics.Debug.WriteLine($"[PostViewer] FileUrl: {_post.FileUrl}");
            
            // Handle media type (image or video)
            if (_post.IsVideo)
            {
                System.Diagnostics.Debug.WriteLine("[PostViewer] Showing video player");
                // Show video player
                VideoPlayer.IsVisible = true;
                PostImage.IsVisible = false;
                
                // Set video source
                CustomVideoPlayer.Source = _post.FileUrl;
                
                // Add video indicator to title
                TitleLabel.Text = $"🎥 Video ({_post.FileExt.ToUpper()})";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[PostViewer] Showing image");
                // Show image
                PostImage.IsVisible = true;
                VideoPlayer.IsVisible = false;
                PostImage.Source = _post.FileUrl;
                
                // Add image indicator to title
                TitleLabel.Text = $"🖼️ Image ({_post.FileExt.ToUpper()})";
            }

            // Set tags
            TagsLabel.Text = _post.Tags;

            // Set details with better formatting
            var details = $"🆔 ID: {_post.Id}\n" +
                         $"👤 Author: {_post.Author}\n" +
                         $"⭐ Score: {_post.Score}\n" +
                         $"📊 Rating: {_post.Rating}\n" +
                         $"📐 Size: {_post.Width} × {_post.Height}\n" +
                         $"💾 File Size: {_post.FileSize / 1024} KB\n" +
                         $"📅 Created: {_post.CreatedAt}\n" +
                         $"📋 Status: {_post.Status}";

            // Add video-specific details
            if (_post.IsVideo)
            {
                details += $"\n🎥 Type: Video ({_post.FileExt.ToUpper()})";
                if (_post.Duration.HasValue)
                {
                    details += $"\n⏱️ Duration: {_post.Duration.Value:F1} seconds";
                }
            }
            else
            {
                details += $"\n🖼️ Type: Image ({_post.FileExt.ToUpper()})";
            }

            DetailsLabel.Text = details;

            // Set source (if available)
            if (!string.IsNullOrEmpty(_post.Source))
            {
                SourceLabel.Text = _post.Source;
                SourceLabel.IsVisible = true;
            }
            else
            {
                SourceLabel.Text = "No source available";
                SourceLabel.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to load post details: {ex.Message}", "OK");
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnOpenFullClicked(object? sender, EventArgs e)
    {
        try
        {
            await Launcher.OpenAsync(new Uri(_post.FileUrl));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to open media: {ex.Message}", "OK");
        }
    }



    private async void OnViewCommentsClicked(object? sender, EventArgs e)
    {
        try
        {
            LoadingOverlay.IsVisible = true;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var comments = await _rule34Service.GetCommentsAsync(_post.Id);
            
            if (comments.Count == 0)
            {
                await DisplayAlert("Comments", "No comments found for this post.", "OK");
            }
            else
            {
                var commentText = string.Join("\n\n", comments.Select(c => 
                    $"💬 By {c.Creator} (Score: {c.Score}):\n{c.Body}"));

                await DisplayAlert("Comments", commentText, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load comments: {ex.Message}", "OK");
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private async void OnSourceTapped(object? sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_post.Source))
        {
            try
            {
                await Launcher.OpenAsync(new Uri(_post.Source));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to open source: {ex.Message}", "OK");
            }
        }
    }
}
