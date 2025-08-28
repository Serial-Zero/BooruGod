using Microsoft.Maui.Controls;

namespace BooruGod.Functions
{
    public static class UiHelper
    {
        public static StackLayout CreatePaginationControls(PaginationHelper pagination, EventHandler? onPreviousClicked = null, EventHandler? onNextClicked = null)
        {
            var paginationLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10,
                Padding = new Thickness(20, 10)
            };

            var previousButton = new Button
            {
                Text = "← Previous",
                IsEnabled = pagination.HasPreviousPage,
                BackgroundColor = pagination.HasPreviousPage ? Colors.Blue : Colors.Gray
            };

            var pageInfoLabel = new Label
            {
                Text = pagination.GetPageInfo(),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 14
            };

            var nextButton = new Button
            {
                Text = "Next →",
                IsEnabled = pagination.HasNextPage,
                BackgroundColor = pagination.HasNextPage ? Colors.Blue : Colors.Gray
            };

            if (onPreviousClicked != null)
                previousButton.Clicked += onPreviousClicked;
            if (onNextClicked != null)
                nextButton.Clicked += onNextClicked;

            paginationLayout.Children.Add(previousButton);
            paginationLayout.Children.Add(pageInfoLabel);
            paginationLayout.Children.Add(nextButton);

            return paginationLayout;
        }

        public static void UpdatePaginationControls(StackLayout paginationLayout, PaginationHelper pagination)
        {
            if (paginationLayout.Children.Count >= 3)
            {
                var previousButton = paginationLayout.Children[0] as Button;
                var pageInfoLabel = paginationLayout.Children[1] as Label;
                var nextButton = paginationLayout.Children[2] as Button;

                if (previousButton != null)
                {
                    previousButton.IsEnabled = pagination.HasPreviousPage;
                    previousButton.BackgroundColor = pagination.HasPreviousPage ? Colors.Blue : Colors.Gray;
                }

                if (pageInfoLabel != null)
                {
                    pageInfoLabel.Text = pagination.GetPageInfo();
                }

                if (nextButton != null)
                {
                    nextButton.IsEnabled = pagination.HasNextPage;
                    nextButton.BackgroundColor = pagination.HasNextPage ? Colors.Blue : Colors.Gray;
                }
            }
        }

        public static Button CreateLoadingButton(string text, EventHandler? onClicked = null)
        {
            var button = new Button
            {
                Text = text,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Colors.Blue,
                TextColor = Colors.White,
                CornerRadius = 5,
                Padding = new Thickness(20, 10)
            };

            if (onClicked != null)
                button.Clicked += onClicked;

            return button;
        }

        public static Label CreateInfoLabel(string text, bool isError = false)
        {
            return new Label
            {
                Text = text,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = isError ? Colors.Red : Colors.Black,
                FontSize = 16,
                Margin = new Thickness(20)
            };
        }
    }
}
