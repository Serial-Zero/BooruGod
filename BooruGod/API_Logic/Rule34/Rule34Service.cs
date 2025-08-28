using Microsoft.Maui.Storage;

namespace BooruGod.API_Logic.Rule34
{
    public class Rule34Service
    {
        private const string ApiKeyPreference = "Rule34ApiKey";
        private const string UserIdPreference = "Rule34UserId";
        
        private Rule34ApiClient? _apiClient;
        private bool _isConfigured = false;

        public bool IsConfigured => _isConfigured;

        public Task<bool> InitializeAsync()
        {
            var apiKey = Preferences.Get(ApiKeyPreference, string.Empty);
            var userId = Preferences.Get(UserIdPreference, string.Empty);

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(userId))
            {
                _isConfigured = false;
                return Task.FromResult(false);
            }

            try
            {
                _apiClient = new Rule34ApiClient(apiKey, userId);
                _isConfigured = true;
                return Task.FromResult(true);
            }
            catch
            {
                _isConfigured = false;
                return Task.FromResult(false);
            }
        }

        public async Task<List<Rule34Post>> GetPostsAsync(string tags = "", int limit = 100, int pid = 0)
        {
            if (!_isConfigured || _apiClient == null)
            {
                throw new InvalidOperationException("Rule34 API is not configured. Please set up your API key and user ID in settings.");
            }

            return await _apiClient.GetPostsAsync(tags, limit, pid);
        }

        public async Task<List<Rule34Post>> GetPostsByIdAsync(int postId)
        {
            if (!_isConfigured || _apiClient == null)
            {
                throw new InvalidOperationException("Rule34 API is not configured. Please set up your API key and user ID in settings.");
            }

            return await _apiClient.GetPostsByIdAsync(postId);
        }

        public async Task<List<Rule34Post>> GetDeletedPostsAsync(int lastId = 0)
        {
            if (!_isConfigured || _apiClient == null)
            {
                throw new InvalidOperationException("Rule34 API is not configured. Please set up your API key and user ID in settings.");
            }

            return await _apiClient.GetDeletedPostsAsync(lastId);
        }

        public async Task<List<Rule34Comment>> GetCommentsAsync(int postId)
        {
            if (!_isConfigured || _apiClient == null)
            {
                throw new InvalidOperationException("Rule34 API is not configured. Please set up your API key and user ID in settings.");
            }

            return await _apiClient.GetCommentsAsync(postId);
        }

        public async Task<List<Rule34Tag>> GetTagsAsync(int limit = 100, int? id = null)
        {
            if (!_isConfigured || _apiClient == null)
            {
                throw new InvalidOperationException("Rule34 API is not configured. Please set up your API key and user ID in settings.");
            }

            return await _apiClient.GetTagsAsync(limit, id);
        }

        public async Task<List<string>> GetAutocompleteAsync(string query)
        {
            if (!_isConfigured || _apiClient == null)
            {
                throw new InvalidOperationException("Rule34 API is not configured. Please set up your API key and user ID in settings.");
            }

            return await _apiClient.GetAutocompleteAsync(query);
        }

        public void Dispose()
        {
            _apiClient?.Dispose();
        }
    }
}
