using System.Text.Json;

namespace BooruGod.API_Logic.Rule34
{
    public class Rule34ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _userId;
        private const string BaseUrl = "https://api.rule34.xxx";

        public Rule34ApiClient(string apiKey, string userId)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            _userId = userId;
        }

        public async Task<List<Rule34Post>> GetPostsAsync(string tags = "", int limit = 100, int pid = 0)
        {
            var url = $"{BaseUrl}/index.php?page=dapi&s=post&q=index&json=1&limit={limit}&pid={pid}&api_key={_apiKey}&user_id={_userId}";
            
            if (!string.IsNullOrEmpty(tags))
            {
                url += $"&tags={Uri.EscapeDataString(tags)}";
            }

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                
                // Debug: Log the full response for analysis
                System.Diagnostics.Debug.WriteLine($"Rule34 API URL: {url}");
                System.Diagnostics.Debug.WriteLine($"Rule34 API Full Response: {response}");
                
                // Check if response is empty or contains error
                if (string.IsNullOrWhiteSpace(response))
                {
                    System.Diagnostics.Debug.WriteLine("API returned empty response");
                    return new List<Rule34Post>();
                }

                // Check if response contains error message
                if (response.Contains("error") || response.Contains("Error") || response.Contains("Missing authentication"))
                {
                    System.Diagnostics.Debug.WriteLine($"API returned error: {response}");
                    return new List<Rule34Post>();
                }

                // Try to parse as direct array first
                try
                {
                    var posts = JsonSerializer.Deserialize<List<Rule34Post>>(response);
                    System.Diagnostics.Debug.WriteLine($"Successfully parsed {posts?.Count ?? 0} posts as direct array");
                    return posts ?? new List<Rule34Post>();
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Direct array parsing failed: {ex.Message}");
                    
                    // If that fails, try to parse as wrapper object
                    try
                    {
                        var wrapper = JsonSerializer.Deserialize<Rule34ResponseWrapper>(response);
                        System.Diagnostics.Debug.WriteLine($"Successfully parsed {wrapper?.Posts?.Count ?? 0} posts as wrapper");
                        return wrapper?.Posts ?? new List<Rule34Post>();
                    }
                    catch (JsonException ex2)
                    {
                        System.Diagnostics.Debug.WriteLine($"Wrapper parsing failed: {ex2.Message}");
                        
                        // Try to parse as a different wrapper format
                        try
                        {
                            var jsonDoc = JsonDocument.Parse(response);
                            System.Diagnostics.Debug.WriteLine($"JSON root element type: {jsonDoc.RootElement.ValueKind}");
                            
                            if (jsonDoc.RootElement.TryGetProperty("post", out var postElement))
                            {
                                var posts = JsonSerializer.Deserialize<List<Rule34Post>>(postElement.GetRawText());
                                System.Diagnostics.Debug.WriteLine($"Successfully parsed {posts?.Count ?? 0} posts from 'post' property");
                                return posts ?? new List<Rule34Post>();
                            }
                            else if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                            {
                                var posts = JsonSerializer.Deserialize<List<Rule34Post>>(response);
                                System.Diagnostics.Debug.WriteLine($"Successfully parsed {posts?.Count ?? 0} posts from array");
                                return posts ?? new List<Rule34Post>();
                            }
                            else
                            {
                                // Log all available properties
                                foreach (var property in jsonDoc.RootElement.EnumerateObject())
                                {
                                    System.Diagnostics.Debug.WriteLine($"Found property: {property.Name} = {property.Value.ValueKind}");
                                }
                            }
                        }
                        catch (Exception ex3)
                        {
                            System.Diagnostics.Debug.WriteLine($"Alternative parsing failed: {ex3.Message}");
                        }
                        
                        // If all parsing attempts fail, return empty list
                        System.Diagnostics.Debug.WriteLine("All parsing attempts failed, returning empty list");
                        return new List<Rule34Post>();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP request failed: {ex.Message}");
                throw new Exception($"Failed to fetch posts: {ex.Message}");
            }
        }

        public async Task<List<Rule34Post>> GetPostsByIdAsync(int postId)
        {
            var url = $"{BaseUrl}/index.php?page=dapi&s=post&q=index&json=1&id={postId}&api_key={_apiKey}&user_id={_userId}";
            
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                
                try
                {
                    var posts = JsonSerializer.Deserialize<List<Rule34Post>>(response);
                    return posts ?? new List<Rule34Post>();
                }
                catch
                {
                    try
                    {
                        var wrapper = JsonSerializer.Deserialize<Rule34ResponseWrapper>(response);
                        return wrapper?.Posts ?? new List<Rule34Post>();
                    }
                    catch
                    {
                        return new List<Rule34Post>();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch post by ID: {ex.Message}");
            }
        }

        public async Task<List<Rule34Post>> GetDeletedPostsAsync(int lastId = 0)
        {
            var url = $"{BaseUrl}/index.php?page=dapi&s=post&q=index&deleted=show&json=1&api_key={_apiKey}&user_id={_userId}";
            
            if (lastId > 0)
            {
                url += $"&last_id={lastId}";
            }

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                
                try
                {
                    var posts = JsonSerializer.Deserialize<List<Rule34Post>>(response);
                    return posts ?? new List<Rule34Post>();
                }
                catch
                {
                    try
                    {
                        var wrapper = JsonSerializer.Deserialize<Rule34ResponseWrapper>(response);
                        return wrapper?.Posts ?? new List<Rule34Post>();
                    }
                    catch
                    {
                        return new List<Rule34Post>();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch deleted posts: {ex.Message}");
            }
        }

        public async Task<List<Rule34Comment>> GetCommentsAsync(int postId)
        {
            var url = $"{BaseUrl}/index.php?page=dapi&s=comment&q=index&json=1&post_id={postId}&api_key={_apiKey}&user_id={_userId}";
            
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var comments = JsonSerializer.Deserialize<List<Rule34Comment>>(response);
                return comments ?? new List<Rule34Comment>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch comments: {ex.Message}");
            }
        }

        public async Task<List<Rule34Tag>> GetTagsAsync(int limit = 100, int? id = null)
        {
            var url = $"{BaseUrl}/index.php?page=dapi&s=tag&q=index&json=1&limit={limit}&api_key={_apiKey}&user_id={_userId}";
            
            if (id.HasValue)
            {
                url += $"&id={id.Value}";
            }

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var tags = JsonSerializer.Deserialize<List<Rule34Tag>>(response);
                return tags ?? new List<Rule34Tag>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch tags: {ex.Message}");
            }
        }

        public async Task<List<string>> GetAutocompleteAsync(string query)
        {
            var url = $"{BaseUrl}/autocomplete.php?q={Uri.EscapeDataString(query)}&api_key={_apiKey}&user_id={_userId}";
            
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var suggestions = JsonSerializer.Deserialize<List<string>>(response);
                return suggestions ?? new List<string>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch autocomplete suggestions: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    // Wrapper class for API responses that might be wrapped in an object
    public class Rule34ResponseWrapper
    {
        public List<Rule34Post> Posts { get; set; } = new List<Rule34Post>();
    }
}
