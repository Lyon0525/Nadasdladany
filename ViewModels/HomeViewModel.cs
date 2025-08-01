using Nadasdladany.Models;

namespace Nadasdladany.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Article> LatestNews { get; set; } = new List<Article>();
        public IEnumerable<Event> UpcomingEvents { get; set; } = new List<Event>();

        public string SiteName { get; set; }
        public string HeroTitle { get; set; }
        public string HeroSubtitle { get; set; }
        public string MayorName { get; set; }
        public string WelcomeTitle { get; set; }
        public string WelcomeMessageParagraph1 { get; set; }
        public string WelcomeMessageParagraph2 { get; set; }
        public string MayorDisplayName { get; set; }
        public string MayorImageUrl { get; set; }
        public string MayorRole { get; set; }
    }
}
