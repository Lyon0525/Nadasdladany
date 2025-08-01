using Nadasdladany.Models;

namespace Nadasdladany.ViewModels
{
    public class AdminMessagesViewModel
    {
        public IEnumerable<ContactSubmission> Messages { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
