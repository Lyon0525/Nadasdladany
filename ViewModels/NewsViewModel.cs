using Nadasdladany.Models;

namespace Nadasdladany.ViewModels
{
    public class NewsIndexViewModel
    {
        public IEnumerable<Article> Articles { get; set; } = new List<Article>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

        public string? CurrentCategorySlug { get; set; }
        public string? CurrentCategoryName { get; set; } // To display the name of the selected category

        // Pagination properties
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public int PageSize { get; set; }
        public int TotalArticles { get; set; }
    }
}
