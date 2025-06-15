using Microsoft.EntityFrameworkCore;
using Nadasdladany.Models;

namespace Nadasdladany.Data
{
    public class NadasdladanyDbContext : DbContext
    {
        public NadasdladanyDbContext(DbContextOptions<NadasdladanyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique slugs for Article
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.Slug)
                .IsUnique();

            // Configure unique slugs for Category
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            // Define relationship between Article and Category
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Or Cascade if appropriate, Restrict prevents deleting category if articles exist

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            var catPolitics = new Category { Id = 1, Name = "Önkormányzati Hírek", Slug = "onkormanyzati-hirek", Description = "Hírek a helyi önkormányzat működéséről." };
            var catCommunity = new Category { Id = 2, Name = "Közösségi Események", Slug = "kozossegi-esemenyek", Description = "Információk a település közösségi programjairól." };
            var catAnnouncements = new Category { Id = 3, Name = "Közlemények", Slug = "kozlemenyek", Description = "Fontos közlemények a lakosság számára." };
            modelBuilder.Entity<Category>().HasData(catPolitics, catCommunity, catAnnouncements);

            // Seed Articles
            modelBuilder.Entity<Article>().HasData(
                new Article
                {
                    Id = 1,
                    Title = "Ünnepi Megemlékezés a Főtéren",
                    Slug = "unnepi-megemlekezes-foteren",
                    Content = "<p>Részletes leírás az ünnepi megemlékezésről, amely a nemzeti ünnepünk alkalmából került megrendezésre a község főterén. Beszédet mondott Varga Tünde polgármester asszony.</p>",
                    Excerpt = "Rövid összefoglaló az ünnepi megemlékezésről, amely a főtéren került megrendezésre.",
                    FeaturedImageUrl = "/images/placeholder/news_placeholder_1.jpg", // Placeholder
                    PublishedDate = DateTime.SpecifyKind(new DateTime(2024, 3, 15, 10, 0, 0), DateTimeKind.Utc),
                    LastModifiedDate = DateTime.SpecifyKind(new DateTime(2024, 3, 15, 10, 0, 0), DateTimeKind.Utc),
                    Author = "Nádasdladány Önkormányzat",
                    IsPublished = true,
                    CategoryId = catCommunity.Id
                },
                new Article
                {
                    Id = 2,
                    Title = "Új Játszótér Átadása a Kossuth Utcában",
                    Slug = "uj-jatszoter-atadasa-kossuth-utcaban",
                    Content = "<p>Az új játszótér átadásának részletei. Modern játékokkal és biztonságos környezettel várjuk a gyermekeket és családjaikat.</p>",
                    Excerpt = "Örömmel jelentjük be, hogy átadásra került a felújított központi játszótér a Kossuth utcában.",
                    FeaturedImageUrl = "/images/placeholder/news_placeholder_2.jpg", // Placeholder
                    PublishedDate = DateTime.SpecifyKind(new DateTime(2024, 4, 22, 14, 0, 0), DateTimeKind.Utc),
                    LastModifiedDate = DateTime.SpecifyKind(new DateTime(2024, 4, 22, 14, 0, 0), DateTimeKind.Utc),
                    Author = "Nádasdladány Önkormányzat",
                    IsPublished = true,
                    CategoryId = catPolitics.Id
                },
                new Article
                {
                    Id = 3,
                    Title = "Közmeghallgatás Időpontja és Témái",
                    Slug = "kozmeghallgatas-idopontja-temai",
                    Content = "<p>Tájékoztatjuk a tisztelt lakosságot, hogy a következő közmeghallgatás időpontja 2024. május 15., 17:00. Helyszín: Művelődési Ház. Témák: éves költségvetés, fejlesztési tervek.</p>",
                    Excerpt = "Fontos információk a következő közmeghallgatásról, melynek fő témái a költségvetés és a fejlesztések lesznek.",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(2024, 5, 1, 9, 0, 0), DateTimeKind.Utc),
                    LastModifiedDate = DateTime.SpecifyKind(new DateTime(2024, 5, 1, 9, 0, 0), DateTimeKind.Utc),
                    Author = "Nádasdladány Önkormányzat",
                    IsPublished = true,
                    CategoryId = catAnnouncements.Id
                }
            );

            // Seed Events (using future dates relative to a fixed point for consistency)
            var baseDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc); // A fixed future base date for seeding

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "Nádasdladányi Falunap 2024",
                    Description = "Egész napos programok kicsiknek és nagyoknak. Koncertek, főzőverseny, kézműves vásár, gyerekprogramok és esti tűzijáték.",
                    StartDate = baseDate.AddDays(15).AddHours(10), // Example: June 16, 2024 10:00
                    EndDate = baseDate.AddDays(15).AddHours(22),   // Example: June 16, 2024 22:00
                    Location = "Községi Sportpálya és Szabadidőpark",
                    Organizer = "Nádasdladány Önkormányzat",
                    EventUrl = "/esemenyek/falunap-2024"
                },
                new Event
                {
                    Id = 2,
                    Title = "Könyvtári Olvasóklub: Nyári Olvasmányok",
                    Description = "Beszélgetés a nyár legnépszerűbb könyveiről, ajánlók és közös élmények megosztása.",
                    StartDate = baseDate.AddDays(25).AddHours(17), // Example: June 26, 2024 17:00
                    EndDate = baseDate.AddDays(25).AddHours(18),   // Example: June 26, 2024 18:00
                    Location = "Helyi Könyvtár Olvasóterme",
                    Organizer = "Nádasdladányi Könyvtár",
                    ContactInfo = "konyvtar@nadasdladany.hu"
                },
                 new Event
                 {
                     Id = 3,
                     Title = "Idősek Napja Ünnepség",
                     Description = "Szeretettel várjuk szépkorú lakosainkat egy közös ünnepségre a Művelődési Ház nagytermében. Ünnepi műsorral és vendéglátással készülünk.",
                     StartDate = baseDate.AddMonths(1).AddDays(10).AddHours(15), // Example: July 11, 2024 15:00
                     Location = "Művelődési Ház",
                     Organizer = "Nádasdladány Önkormányzat és a Helyi Nyugdíjas Klub"
                 }
            );
        }
    }
}
