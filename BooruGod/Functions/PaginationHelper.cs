namespace BooruGod.Functions
{
    public class PaginationHelper
    {
        public int CurrentPage { get; private set; } = 0;
        public int PageSize { get; private set; } = 50;
        public int TotalItems { get; private set; } = 0;
        public bool HasNextPage => (CurrentPage + 1) * PageSize < TotalItems;
        public bool HasPreviousPage => CurrentPage > 0;

        public PaginationHelper(int pageSize = 50)
        {
            PageSize = pageSize;
        }

        public void SetTotalItems(int total)
        {
            TotalItems = total;
        }

        public void NextPage()
        {
            if (HasNextPage)
            {
                CurrentPage++;
            }
        }

        public void PreviousPage()
        {
            if (HasPreviousPage)
            {
                CurrentPage--;
            }
        }

        public void ResetToFirstPage()
        {
            CurrentPage = 0;
        }

        public void GoToPage(int page)
        {
            if (page >= 0 && page * PageSize < TotalItems)
            {
                CurrentPage = page;
            }
        }

        public int GetStartIndex()
        {
            return CurrentPage * PageSize;
        }

        public int GetEndIndex()
        {
            return Math.Min((CurrentPage + 1) * PageSize - 1, TotalItems - 1);
        }

        public string GetPageInfo()
        {
            var start = GetStartIndex() + 1;
            var end = GetEndIndex() + 1;
            return $"Page {CurrentPage + 1} ({start}-{end} of {TotalItems})";
        }

        public void UpdatePageSize(int newPageSize)
        {
            PageSize = newPageSize;
            ResetToFirstPage();
        }
    }
}
