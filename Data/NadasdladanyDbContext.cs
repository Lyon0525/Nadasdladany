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
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentCategory> DocumentCategories { get; set; }
        public DbSet<ContactSubmission> ContactSubmissions { get; set; } // If you decided to store contact messages
        public DbSet<OfficeInfo> OfficeInfos { get; set; }
        public DbSet<OfficeHourEntry> OfficeHourEntries { get; set; }
        public DbSet<Representative> Representatives { get; set; }
        public DbSet<Institution> Institutions { get; set; } // <<-- ADD THIS DbSet

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

            modelBuilder.Entity<Institution>()
               .HasIndex(i => i.Slug)
               .IsUnique(); // Ensure slugs are unique if used for routing

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
                    StartDate = baseDate.AddDays(15).AddHours(10), // Example: June 16, 10:00 (Kind will be Utc)
                    EndDate = baseDate.AddDays(15).AddHours(22),   // Example: June 16, 22:00 (Kind will be Utc)
                    Location = "Községi Sportpálya és Szabadidőpark",
                    Organizer = "Nádasdladány Önkormányzat",
                    EventUrl = "/esemenyek/falunap-2024", // This EventUrl is relative, might be better as full or handled differently.
                                                          // Or just use Slug for routing and EventUrl for external links.
                    IsAllDay = false, // Explicitly set if your model defaults to false
                    IsPublished = true,
                    Slug = "nadasdladanyi-falunap-" + baseDate.AddDays(15).Year // Example slug
                },
                new Event
                {
                    Id = 2,
                    Title = "Könyvtári Olvasóklub: Nyári Olvasmányok",
                    Description = "Beszélgetés a nyár legnépszerűbb könyveiről, ajánlók és közös élmények megosztása.",
                    StartDate = baseDate.AddDays(25).AddHours(17), // Example: June 26, 17:00
                    EndDate = baseDate.AddDays(25).AddHours(18),   // Example: June 26, 18:00
                    Location = "Helyi Könyvtár Olvasóterme",
                    Organizer = "Nádasdladányi Könyvtár",
                    ContactInfo = "konyvtar@nadasdladany.hu",
                    IsAllDay = false,
                    IsPublished = true,
                    Slug = "konyvtari-olvasoklub-nyari-olvasmanyok" // Example slug
                },
                 new Event
                 {
                     Id = 3,
                     Title = "Idősek Napja Ünnepség",
                     Description = "Szeretettel várjuk szépkorú lakosainkat egy közös ünnepségre a Művelődési Ház nagytermében. Ünnepi műsorral és vendéglátással készülünk.",
                     StartDate = baseDate.AddMonths(1).AddDays(10).AddHours(15), // Example: July 11, 15:00
                     // EndDate = null, // If it's a single point in time or duration isn't fixed.
                     Location = "Művelődési Ház",
                     Organizer = "Nádasdladány Önkormányzat és a Helyi Nyugdíjas Klub",
                     IsAllDay = false,
                     IsPublished = true,
                     Slug = "idosek-napja-unnepseg-" + baseDate.AddMonths(1).Year // Example slug
                 },
                  new Event
                  {
                      Id = 4, // Ensure this ID is unique among seeded events
                      Title = "Nádasdladányi Falunap 2026",
                      Description = "Nagyszabású falunap sok meglepetéssel, hagyományőrző programokkal, esti koncerttel és tűzijátékkal ünnepeljük községünket!",
                      StartDate = new DateTime(2026, 6, 9, 10, 0, 0, DateTimeKind.Utc), // June 9, 2026, 10:00 AM UTC
                      EndDate = new DateTime(2026, 6, 9, 23, 0, 0, DateTimeKind.Utc),   // June 9, 2026, 11:00 PM UTC
                      Location = "Központi Rendezvénytér",
                      Organizer = "Nádasdladány Önkormányzata",
                      EventUrl = null, // No specific URL yet, or could be like "/esemenyek/falunap-2026"
                      IsAllDay = false,
                      IsPublished = true, // Make sure it's published to be visible
                      Slug = "nadasdladanyi-falunap-2026" // Create a unique slug
                  }
            );
            modelBuilder.Entity<DocumentCategory>()
                .HasIndex(dc => dc.Slug)
                .IsUnique();

            var catRendeletek = new DocumentCategory { Id = 1, Name = "Rendeletek", Slug = "rendeletek", Description = "Önkormányzati rendeletek gyűjteménye." };
            var catJegyzokonyvek = new DocumentCategory { Id = 2, Name = "Jegyzőkönyvek", Slug = "jegyzokonyvek", Description = "Képviselő-testületi és bizottsági ülések jegyzőkönyvei." };
            var catHatarozatok = new DocumentCategory { Id = 3, Name = "Határozatok", Slug = "hatarozatok", Description = "A Képviselő-testület és a polgármester határozatai." };
            var catKozerdeku = new DocumentCategory { Id = 4, Name = "Közérdekű Adatok", Slug = "kozerdeku-adatok", Description = "Törvény által kötelezően közzéteendő közérdekű adatok." };
            var catPalyazatok = new DocumentCategory { Id = 5, Name = "Pályázatok és Beszámolók", Slug = "palyazatok-beszamolok", Description = "Elnyert pályázatok és azokhoz kapcsolódó dokumentumok, beszámolók." };
            modelBuilder.Entity<DocumentCategory>().HasData(catRendeletek, catJegyzokonyvek, catHatarozatok, catKozerdeku, catPalyazatok);


            // --- Document Configuration & Seed (Example) ---
            modelBuilder.Entity<Document>()
                .HasOne(d => d.DocumentCategory)
                .WithMany(dc => dc.Documents)
                .HasForeignKey(d => d.DocumentCategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting category if documents are linked

            modelBuilder.Entity<Document>().HasData(
                new Document
                {
                    Id = 1,
                    Title = "2024. évi költségvetési rendelet",
                    Description = "Nádasdladány község 2024. évi költségvetéséről szóló 1/2024. (II.15.) önkormányzati rendelet.",
                    FilePath = "documents/rendeletek/2024/1-2024-koltsegvetes.pdf", // Relative path from a wwwroot base
                    FileType = "PDF",
                    FileSizeInBytes = 123456,
                    UploadedDate = DateTime.SpecifyKind(new DateTime(2024, 2, 15), DateTimeKind.Utc),
                    IsPublished = true,
                    DocumentCategoryId = catRendeletek.Id,
                    LastModifiedDate = DateTime.SpecifyKind(new DateTime(2024, 2, 15), DateTimeKind.Utc)
                },
                new Document
                {
                    Id = 2,
                    Title = "2024. március 10-i testületi ülés jegyzőkönyve",
                    Description = "A Képviselő-testület rendes ülésének jegyzőkönyve.",
                    FilePath = "documents/jegyzokonyvek/2024/2024-03-10-testuleti-ules.pdf",
                    FileType = "PDF",
                    FileSizeInBytes = 234567,
                    UploadedDate = DateTime.SpecifyKind(new DateTime(2024, 3, 12), DateTimeKind.Utc),
                    IsPublished = true,
                    DocumentCategoryId = catJegyzokonyvek.Id,
                    LastModifiedDate = DateTime.SpecifyKind(new DateTime(2024, 3, 12), DateTimeKind.Utc)
                },
                new Document
                {
                    Id = 3,
                    Title = "Adatkezelési Tájékoztató",
                    Description = "Az önkormányzat adatkezelési gyakorlatáról szóló hivatalos tájékoztató.",
                    FilePath = "documents/kozerdeku/adatkezelesi-tajekoztato-2024.pdf",
                    FileType = "PDF",
                    FileSizeInBytes = 98765,
                    UploadedDate = DateTime.SpecifyKind(new DateTime(2024, 1, 20), DateTimeKind.Utc),
                    IsPublished = true,
                    DocumentCategoryId = catKozerdeku.Id,
                    LastModifiedDate = DateTime.SpecifyKind(new DateTime(2024, 1, 20), DateTimeKind.Utc)
                }
            );
            modelBuilder.Entity<OfficeInfo>().HasData(new OfficeInfo
            {
                Id = 1, // Fixed ID for the single office info record
                OfficeName = "Nádasdladányi Polgármesteri Hivatal",
                AboutOffice = "<p>A Nádasdladányi Polgármesteri Hivatal felelős a helyi önkormányzati feladatok ellátásáért, a képviselő-testület döntéseinek végrehajtásáért, valamint a lakossági ügyintézésért. Célunk a hatékony, átlátható és ügyfélbarát közigazgatás biztosítása.</p><p>Munkatársaink készséggel állnak rendelkezésükre a különböző ügytípusokban, legyen szó adóügyekről, szociális támogatásokról, anyakönyvi kivonatokról vagy helyi engedélyekről.</p>",
                Address = "8145 Nádasdladány, Fő utca 1.",
                PhoneNumber = "+36 (22) 123-456",
                Email = "hivatal@nadasdladany.hu",
                WebsiteUrl = "https://www.nadasdladany.hu", // Or keep null if same as site
                GoogleMapsEmbedUrl = "<iframe src=\"https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d2710.894867318367!2d18.23535761561613!3d47.20190097915973!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x4769eb3b71697cc9%3A0x3a905520d403d2b9!2sN%C3%A1dasdlad%C3%A1ny%2C%20F%C5%91%20utca%201!5e0!3m2!1shu!2shu!4v1620000000000!5m2!1shu!2shu\" width=\"600\" height=\"450\" style=\"border:0;\" allowfullscreen=\"\" loading=\"lazy\"></iframe>" // Example embed code
            });

            // --- OfficeHourEntry Seed ---
            modelBuilder.Entity<OfficeHourEntry>().HasData(
                new OfficeHourEntry { Id = 1, DayOfWeek = DayOfWeek.Monday, TimeDescription = "8:00 - 12:00 és 13:00 - 16:00", DisplayOrder = 1 },
                new OfficeHourEntry { Id = 2, DayOfWeek = DayOfWeek.Tuesday, TimeDescription = "Nincs ügyfélfogadás", DisplayOrder = 2 },
                new OfficeHourEntry { Id = 3, DayOfWeek = DayOfWeek.Wednesday, TimeDescription = "8:00 - 12:00 és 13:00 - 17:00 (Hosszított)", DisplayOrder = 3 },
                new OfficeHourEntry { Id = 4, DayOfWeek = DayOfWeek.Thursday, TimeDescription = "Nincs ügyfélfogadás", DisplayOrder = 4 },
                new OfficeHourEntry { Id = 5, DayOfWeek = DayOfWeek.Friday, TimeDescription = "8:00 - 12:00", DisplayOrder = 5 },
                new OfficeHourEntry { Id = 6, DayOfWeek = DayOfWeek.Saturday, TimeDescription = "Zárva", DisplayOrder = 6 },
                new OfficeHourEntry { Id = 7, DayOfWeek = DayOfWeek.Sunday, TimeDescription = "Zárva", DisplayOrder = 7 }
            );

            // --- Representative Seed ---
            modelBuilder.Entity<Representative>().HasData(
                new Representative
                {
                    Id = 1,
                    Name = "Varga Tünde",
                    Role = RepresentativeRole.Polgarmester,
                    Email = "polgarmester@nadasdladany.hu",
                    PhoneNumber = "+36 (30) 111-2233",
                    ReceptionHoursInfo = "Minden hónap első szerdáján 14:00-16:00 (Előzetes bejelentkezés szükséges)",
                    ImageUrl = "/images/representatives/mayor-varga-tunde.jpg", // Placeholder
                    Biography = "<p>Tisztelt Nádasdladányiak! Polgármesterként legfőbb célom községünk fejlődése és közösségünk erősítése. Nyitott ajtókkal várom Önöket!</p>",
                    IsPublished = true,
                    DisplayOrder = 1
                },
                new Representative
                {
                    Id = 2,
                    Name = "Dr. Kovács István",
                    Role = RepresentativeRole.Alpolgarmester,
                    CustomTitleOverride = "Alpolgármester, Pénzügyi és Ügyrendi Bizottság Elnöke",
                    Email = "alpolgarmester@nadasdladany.hu",
                    ImageUrl = "/images/representatives/kovacs-istvan.jpg", // Placeholder
                    IsPublished = true,
                    DisplayOrder = 2
                },
                new Representative
                {
                    Id = 3,
                    Name = "Nagy Mária",
                    Role = RepresentativeRole.Kepviselo,
                    CustomTitleOverride = "Képviselő, Szociális és Kulturális Bizottság Tagja",
                    ImageUrl = "/images/representatives/nagy-maria.jpg", // Placeholder
                    IsPublished = true,
                    DisplayOrder = 3
                },
                new Representative
                {
                    Id = 4,
                    Name = "Horváth Géza",
                    Role = RepresentativeRole.Jegyző,
                    CustomTitleOverride = "Jegyző",
                    Email = "jegyzo@nadasdladany.hu",
                    PhoneNumber = "+36 (22) 123-457",
                    ImageUrl = "/images/representatives/horvath-geza-jegyzo.jpg", // Placeholder
                    IsPublished = true,
                    DisplayOrder = 10 // Jegyző usually listed separately or lower
                }
            );

            modelBuilder.Entity<Institution>().HasData(
                new Institution
                {
                    Id = 1,
                    Name = "Nádasdy Ferenc Általános Iskola",
                    Slug = "nadasdy-ferenc-altalanos-iskola",
                    Description = "Községünk általános iskolája, amely alapfokú oktatást biztosít a helyi és környékbeli gyermekek számára.",
                    Address = "Nádasdladány, Iskola utca 1. (P)", // (P) for placeholder
                    PhoneNumber = "+36 (22) 234-567 (P)",
                    Email = "iskola@nadasdladany.edu.hu (P)",
                    WebsiteUrl = "http://www.nadasdyiskola-nl.hu/", // Placeholder or actual
                    ImageUrl = "/images/institutions/iskola-placeholder.jpg",
                    IconCssClass = "bi bi-book-fill",
                    IsPublished = true,
                    DisplayOrder = 10
                },
                new Institution
                {
                    Id = 2,
                    Name = "Napraforgó Óvoda és Bölcsőde",
                    Slug = "napraforgo-ovoda-es-bolcsode",
                    Description = "Szeretetteljes és biztonságos környezetben neveljük a legkisebbeket, felkészítve őket az iskolás évekre.",
                    Address = "Nádasdladány, Óvoda köz 2. (P)",
                    PhoneNumber = "+36 (22) 345-678 (P)",
                    Email = "ovoda@nadasdladany.edu.hu (P)",
                    ImageUrl = "/images/institutions/ovoda-placeholder.jpg",
                    IconCssClass = "bi bi-palette-fill",
                    IsPublished = true,
                    DisplayOrder = 20
                },
                new Institution
                {
                    Id = 3,
                    Name = "Művelődési Ház és Könyvtár",
                    Slug = "muvelodesi-haz-es-konyvtar",
                    Description = "Közösségi programok, kulturális események, kiállítások és könyvtári szolgáltatások helyszíne.",
                    Address = "Nádasdladány, Kultúra tér 3. (P)",
                    PhoneNumber = "+36 (22) 456-789 (P)",
                    Email = "muvhaz@nadasdladany.info (P)",
                    WebsiteUrl = "http://www.nadasdladanyimuvelodesihaz.hu", // Placeholder
                    ImageUrl = "/images/institutions/muvhaz-placeholder.jpg",
                    IconCssClass = "bi bi-collection-play-fill",
                    IsPublished = true,
                    DisplayOrder = 30
                },
                new Institution
                {
                    Id = 4,
                    Name = "Orvosi Rendelő (Háziorvos)",
                    Slug = "orvosi-rendelo-haziorvos",
                    Description = "Háziorvosi és gyermekorvosi ellátás a település lakosai számára.",
                    Address = "Nádasdladány, Egészség út 4. (P)",
                    PhoneNumber = "+36 (22) 567-890 (P)",
                    ImageUrl = "/images/institutions/orvosi-rendelo-placeholder.jpg",
                    IconCssClass = "bi bi-heart-pulse-fill",
                    IsPublished = true,
                    DisplayOrder = 40
                },
                new Institution
                {
                    Id = 5,
                    Name = "Fogorvosi Rendelő",
                    Slug = "fogorvosi-rendelo",
                    Description = "Fogászati alapellátás és szakellátás.",
                    Address = "Nádasdladány, Egészség út 4/B. (P)",
                    PhoneNumber = "+36 (22) 678-901 (P)",
                    ImageUrl = "/images/institutions/fogorvosi-rendelo-placeholder.jpg",
                    IconCssClass = "bi bi-bandaid-fill",
                    IsPublished = true,
                    DisplayOrder = 50
                }
                // Add more seed data as needed for Védőnői Szolgálat, etc.
            );
        }
    }
}
