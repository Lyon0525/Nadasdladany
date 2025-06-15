using Nadasdladany.Models;

namespace Nadasdladany.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Article> LatestNews { get; set; } = new List<Article>();
        public IEnumerable<Event> UpcomingEvents { get; set; } = new List<Event>();

        // Data for Hero Banner and Welcome Message (can be configured or hardcoded as examples)
        public string SiteName { get; set; } = "Nádasdladány Község Honlapja";
        public string HeroTitle { get; set; } = "Üdvözöljük Nádasdladány Honlapján!";
        public string HeroSubtitle { get; set; } = "Fedezze fel községünk életét, híreit, intézményeit és látnivalóit. Legyen naprakész a helyi eseményekkel kapcsolatban!";
        public string MayorName { get; set; } = "Varga Tünde";
        public string WelcomeTitle { get; set; } = "Tisztelt Látogató!";
        public string WelcomeMessageParagraph1 { get; set; } = "Szeretettel köszöntöm Önt Nádasdladány község hivatalos honlapján! Célunk, hogy ezen a felületen keresztül átfogó képet adjunk településünk mindennapjairól, működéséről, valamint lehetőséget biztosítsunk az egyszerű és gyors tájékozódásra.";
        public string WelcomeMessageParagraph2 { get; set; } = "Böngésszen híreink között, ismerje meg önkormányzatunk munkáját, intézményeinket és fedezze fel Nádasdladány természeti és épített értékeit. Reméljük, hasznos információkkal szolgálhatunk minden kedves érdeklődő számára.";
    }
}
