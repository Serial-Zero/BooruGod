using Microsoft.Maui.Controls;

namespace BooruGod.Functions.MediaPlayer
{
    public class VideoPlayer : ContentView
    {
        private WebView _webView;

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            nameof(Source), typeof(string), typeof(VideoPlayer), null, propertyChanged: OnSourceChanged);

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public VideoPlayer()
        {
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            _webView = new WebView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            Content = _webView;
        }

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is VideoPlayer player && newValue is string source)
            {
                var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{ 
            margin: 0; 
            padding: 0; 
            background: #000; 
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }}
        video {{ 
            width: 100%; 
            height: 100vh; 
            object-fit: contain;
            background: #000;
            max-width: 100%;
            max-height: 100vh;
        }}
        .video-container {{
            width: 100%;
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }}
    </style>
</head>
<body>
    <div class='video-container'>
        <video controls preload='metadata' playsinline>
            <source src='{source}' type='video/mp4'>
            <source src='{source}' type='video/webm'>
            <source src='{source}' type='video/ogg'>
            Your browser does not support the video tag.
        </video>
    </div>
    <script>
        document.addEventListener('DOMContentLoaded', function() {{
            const video = document.querySelector('video');
            if (video) {{
                video.addEventListener('loadedmetadata', function() {{
                    console.log('Video metadata loaded');
                }});
                video.addEventListener('canplay', function() {{
                    console.log('Video can start playing');
                }});
                video.addEventListener('error', function(e) {{
                    console.error('Video error:', e);
                }});
            }}
        }});
    </script>
</body>
</html>";
                
                player._webView.Source = new HtmlWebViewSource { Html = htmlContent };
            }
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            if (Handler == null)
            {
                _webView?.EvaluateJavaScriptAsync("document.querySelector('video').pause();");
            }
        }
    }
}
