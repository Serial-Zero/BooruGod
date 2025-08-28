using System.Text.Json.Serialization;

namespace BooruGod.API_Logic.Rule34
{
    public class Rule34Post
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("tags")]
        public string Tags { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; } = string.Empty;

        [JsonPropertyName("creator_id")]
        public int CreatorId { get; set; }

        [JsonPropertyName("approver_id")]
        public int? ApproverId { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;

        [JsonPropertyName("change")]
        public int Change { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("md5")]
        public string Md5 { get; set; } = string.Empty;

        [JsonPropertyName("file_size")]
        public long FileSize { get; set; }

        [JsonPropertyName("file_url")]
        public string FileUrl { get; set; } = string.Empty;

        [JsonPropertyName("is_shown_in_index")]
        public bool IsShownInIndex { get; set; }

        [JsonPropertyName("preview_url")]
        public string PreviewUrl { get; set; } = string.Empty;

        [JsonPropertyName("preview_width")]
        public int PreviewWidth { get; set; }

        [JsonPropertyName("preview_height")]
        public int PreviewHeight { get; set; }

        [JsonPropertyName("actual_preview_width")]
        public int ActualPreviewWidth { get; set; }

        [JsonPropertyName("actual_preview_height")]
        public int ActualPreviewHeight { get; set; }

        [JsonPropertyName("sample_url")]
        public string SampleUrl { get; set; } = string.Empty;

        [JsonPropertyName("sample_width")]
        public int SampleWidth { get; set; }

        [JsonPropertyName("sample_height")]
        public int SampleHeight { get; set; }

        [JsonPropertyName("sample_file_size")]
        public long SampleFileSize { get; set; }

        [JsonPropertyName("jpeg_url")]
        public string JpegUrl { get; set; } = string.Empty;

        [JsonPropertyName("jpeg_width")]
        public int JpegWidth { get; set; }

        [JsonPropertyName("jpeg_height")]
        public int JpegHeight { get; set; }

        [JsonPropertyName("jpeg_file_size")]
        public long JpegFileSize { get; set; }

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonPropertyName("is_rating_locked")]
        public bool IsRatingLocked { get; set; }

        [JsonPropertyName("has_children")]
        public bool HasChildren { get; set; }

        [JsonPropertyName("parent_id")]
        public int? ParentId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("is_pending")]
        public bool IsPending { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("is_held")]
        public bool IsHeld { get; set; }

        [JsonPropertyName("frames_pending_string")]
        public string FramesPendingString { get; set; } = string.Empty;

        [JsonPropertyName("frames_pending")]
        public List<string> FramesPending { get; set; } = new List<string>();

        [JsonPropertyName("frames_string")]
        public string FramesString { get; set; } = string.Empty;

        [JsonPropertyName("frames")]
        public List<string> Frames { get; set; } = new List<string>();

        [JsonPropertyName("flag_detail")]
        public FlagDetail? FlagDetail { get; set; }

        [JsonPropertyName("relationships")]
        public Relationships? Relationships { get; set; }

        [JsonPropertyName("is_note_locked")]
        public bool IsNoteLocked { get; set; }

        [JsonPropertyName("last_noted_at")]
        public int LastNotedAt { get; set; }

        [JsonPropertyName("last_commented_at")]
        public int LastCommentedAt { get; set; }

        [JsonPropertyName("file_ext")]
        public string FileExt { get; set; } = string.Empty;

        [JsonPropertyName("duration")]
        public double? Duration { get; set; }

        [JsonPropertyName("animated")]
        public bool Animated { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; } = string.Empty;

        public bool IsVideo => (!string.IsNullOrEmpty(FileExt) && (FileExt.ToLower() == "webm" || FileExt.ToLower() == "mp4" || FileExt.ToLower() == "avi" || FileExt.ToLower() == "mov" || FileExt.ToLower() == "gif")) || 
                               (!string.IsNullOrEmpty(Image) && (Image.ToLower().EndsWith(".mp4") || Image.ToLower().EndsWith(".webm") || Image.ToLower().EndsWith(".avi") || Image.ToLower().EndsWith(".mov") || Image.ToLower().EndsWith(".gif")));
    }

    public class FlagDetail
    {
        [JsonPropertyName("post_id")]
        public int PostId { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; } = string.Empty;

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("flag_type")]
        public string FlagType { get; set; } = string.Empty;

        [JsonPropertyName("is_resolved")]
        public bool IsResolved { get; set; }

        [JsonPropertyName("resolver_id")]
        public int? ResolverId { get; set; }
    }

    public class Relationships
    {
        [JsonPropertyName("parent_id")]
        public int? ParentId { get; set; }

        [JsonPropertyName("has_children")]
        public bool HasChildren { get; set; }

        [JsonPropertyName("has_active_children")]
        public bool HasActiveChildren { get; set; }

        [JsonPropertyName("children")]
        public List<int> Children { get; set; } = new List<int>();
    }

    public class Rule34Comment
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        [JsonPropertyName("post_id")]
        public int PostId { get; set; }

        [JsonPropertyName("creator")]
        public string Creator { get; set; } = string.Empty;

        [JsonPropertyName("creator_id")]
        public int CreatorId { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public int Score { get; set; }
    }

    public class Rule34Tag
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("ambiguous")]
        public bool Ambiguous { get; set; }
    }
}
