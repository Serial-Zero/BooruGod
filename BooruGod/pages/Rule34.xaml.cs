using BooruGod.API_Logic.Rule34;
using BooruGod.Functions;
using System.Collections.ObjectModel;

namespace BooruGod.pages;

public partial class Rule34 : ContentPage
{
    private readonly Rule34Service _rule34Service;
    private readonly PaginationHelper _pagination;
    private ObservableCollection<Rule34Post> _posts;
    private string _currentSearch = "";

    public ObservableCollection<Rule34Post> Posts
    {
        get => _posts;
        set
        {
            _posts = value;
            OnPropertyChanged();
        }
    }

    public Rule34()
    {
        InitializeComponent();
        _rule34Service = new Rule34Service();
        _pagination = new PaginationHelper(50);
        _posts = new ObservableCollection<Rule34Post>();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CheckAuthenticationAndLoadContent();
    }

    private async Task CheckAuthenticationAndLoadContent()
    {
        System.Diagnostics.Debug.WriteLine("Checking authentication...");
        var isConfigured = await _rule34Service.InitializeAsync();
        System.Diagnostics.Debug.WriteLine($"Authentication result: {isConfigured}");
        
        if (isConfigured)
        {
            System.Diagnostics.Debug.WriteLine("Showing authenticated view");
            NotAuthenticatedView.IsVisible = false;
            AuthenticatedView.IsVisible = true;
            await LoadImages();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Showing not authenticated view");
            NotAuthenticatedView.IsVisible = true;
            AuthenticatedView.IsVisible = false;
        }
    }

    private async Task LoadImages()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Loading images with search: '{_currentSearch}', page: {_pagination.CurrentPage}");
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var posts = await _rule34Service.GetPostsAsync(_currentSearch, _pagination.PageSize, _pagination.CurrentPage);
            System.Diagnostics.Debug.WriteLine($"Received {posts.Count} posts from API");
            
            Posts.Clear();
            foreach (var post in posts)
            {
                Posts.Add(post);
            }
            
            // Update pagination info (assuming we have more pages if we got a full page)
            _pagination.SetTotalItems(posts.Count >= _pagination.PageSize ? (_pagination.CurrentPage + 1) * _pagination.PageSize + 100 : (_pagination.CurrentPage * _pagination.PageSize) + posts.Count);
            UpdatePaginationControls();
            
            System.Diagnostics.Debug.WriteLine($"Added {Posts.Count} posts to collection");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading images: {ex.Message}");
            await DisplayAlert("Error", $"Failed to load images: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }

    private void UpdatePaginationControls()
    {
        PreviousButton.IsEnabled = _pagination.HasPreviousPage;
        PreviousButton.BackgroundColor = _pagination.HasPreviousPage ? Colors.Blue : Colors.Gray;
        
        PageInfoLabel.Text = _pagination.GetPageInfo();
        
        NextButton.IsEnabled = _pagination.HasNextPage;
        NextButton.BackgroundColor = _pagination.HasNextPage ? Colors.Blue : Colors.Gray;
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        _currentSearch = SearchEntry.Text?.Trim() ?? "";
        _pagination.ResetToFirstPage();
        System.Diagnostics.Debug.WriteLine($"Search clicked with term: '{_currentSearch}'");
        await LoadImages();
    }

    private async void OnPreviousPageClicked(object? sender, EventArgs e)
    {
        if (_pagination.HasPreviousPage)
        {
            _pagination.PreviousPage();
            await LoadImages();
        }
    }

    private async void OnNextPageClicked(object? sender, EventArgs e)
    {
        if (_pagination.HasNextPage)
        {
            _pagination.NextPage();
            await LoadImages();
        }
    }

    private async void OnImageSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Rule34Post selectedPost)
        {
            ImagesCollectionView.SelectedItem = null;
            
            // Navigate to PostViewer page
            await Navigation.PushAsync(new PostViewer(selectedPost, _rule34Service));
        }
    }

    private async void OnGoToSettingsClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new Settings());
    }
}
