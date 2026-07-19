using System;
using System.Collections.Generic;
using System.Text;

namespace BankBranchManagementSystem.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public string? SearchTerm { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Returns the page numbers to render in pagination controls, collapsing
        /// long runs into a single null entry (rendered as "..." in the view).
        /// Always shows page 1, the last page, and a window around the current page.
        /// </summary>
        public List<int?> GetPageNumbers(int siblingCount = 1)
        {
            var pages = new List<int?>();

            if (TotalPages <= 10)
            {
                for (int i = 1; i <= TotalPages; i++)
                    pages.Add(i);
                return pages;
            }

            pages.Add(1);

            int start = Math.Max(2, PageNumber - siblingCount);
            int end = Math.Min(TotalPages - 1, PageNumber + siblingCount);

            if (start > 2)
                pages.Add(null); // ellipsis after page 1

            for (int i = start; i <= end; i++)
                pages.Add(i);

            if (end < TotalPages - 1)
                pages.Add(null); // ellipsis before last page

            pages.Add(TotalPages);

            return pages;
        }
    }
}