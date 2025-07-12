using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Models;

namespace Nadasdladany.Data
{
    public class NadasdladanyDbContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<ContactSubmission> ContactSubmissions { get; set; } 
        public DbSet<OfficeInfo> OfficeInfos { get; set; }
        public DbSet<OfficeHourEntry> OfficeHourEntries { get; set; }
        public DbSet<Representative> Representatives { get; set; }
        public DbSet<Institution> Institutions { get; set; } 
        public DbSet<GalleryImage> GalleryImages { get; set; }   
        public DbSet<GalleryAlbum> GalleryAlbums { get; set; }
        public DbSet<UsefulLink> UsefulLinks { get; set; } 

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
            DateTime fixedReferenceDateForArticles = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime fixedReferenceDateForEvents = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc); // Fixed future base
            DateTime fixedReferenceDateForGallery = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
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
                   Content = "<p>...</p>",
                   Excerpt = "...",
                   FeaturedImageUrl = "/images/placeholder/news_placeholder_1.jpg",
                   PublishedDate = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc), // Fixed
                   LastModifiedDate = new DateTime(2024, 3, 15, 10, 0, 0, DateTimeKind.Utc), // Fixed
                   Author = "Nádasdladány Önkormányzat",
                   IsPublished = true,
                   CategoryId = catCommunity.Id
               },
               new Article
               {
                   Id = 2,
                   Title = "Új Játszótér Átadása a Kossuth Utcában",
                   Slug = "uj-jatszoter-atadasa-kossuth-utcaban",
                   Content = "<p>...</p>",
                   Excerpt = "...",
                   FeaturedImageUrl = "/images/placeholder/news_placeholder_2.jpg",
                   PublishedDate = new DateTime(2024, 4, 22, 14, 0, 0, DateTimeKind.Utc), // Fixed
                   LastModifiedDate = new DateTime(2024, 4, 22, 14, 0, 0, DateTimeKind.Utc), // Fixed
                   Author = "Nádasdladány Önkormányzat",
                   IsPublished = true,
                   CategoryId = catPolitics.Id
               },
               new Article
               {
                   Id = 3,
                   Title = "Közmeghallgatás Időpontja és Témái",
                   Slug = "kozmeghallgatas-idopontja-temai",
                   Content = "<p>...</p>",
                   Excerpt = "...",
                   PublishedDate = new DateTime(2024, 5, 1, 9, 0, 0, DateTimeKind.Utc), // Fixed
                   LastModifiedDate = new DateTime(2024, 5, 1, 9, 0, 0, DateTimeKind.Utc), // Fixed
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
                   Description = "...",
                   StartDate = fixedReferenceDateForEvents.AddDays(15).AddHours(10), // June 16, 2024 10:00 UTC
                   EndDate = fixedReferenceDateForEvents.AddDays(15).AddHours(22),   // June 16, 2024 22:00 UTC
                   Location = "Községi Sportpálya és Szabadidőpark",
                   Organizer = "Nádasdladány Önkormányzat",
                   EventUrl = "/esemenyek/falunap-2024",
                   IsAllDay = false,
                   IsPublished = true,
                   Slug = "nadasdladanyi-falunap-2024" // Fixed slug
               },
               new Event
               {
                   Id = 2,
                   Title = "Könyvtári Olvasóklub: Nyári Olvasmányok 2024",
                   Description = "...",
                   StartDate = fixedReferenceDateForEvents.AddDays(25).AddHours(17), // June 26, 2024 17:00 UTC
                   EndDate = fixedReferenceDateForEvents.AddDays(25).AddHours(18),   // June 26, 2024 18:00 UTC
                   Location = "Helyi Könyvtár Olvasóterme",
                   Organizer = "Nádasdladányi Könyvtár",
                   ContactInfo = "konyvtar@nadasdladany.hu",
                   IsAllDay = false,
                   IsPublished = true,
                   Slug = "konyvtari-olvasoklub-nyari-olvasmanyok-2024" // Fixed slug
               },
                new Event
                {
                    Id = 3,
                    Title = "Idősek Napja Ünnepség 2024",
                    Description = "...",
                    StartDate = fixedReferenceDateForEvents.AddMonths(1).AddDays(10).AddHours(15), // July 11, 2024 15:00 UTC
                    Location = "Művelődési Ház",
                    Organizer = "Nádasdladány Önkormányzat és a Helyi Nyugdíjas Klub",
                    IsAllDay = false,
                    IsPublished = true,
                    Slug = "idosek-napja-unnepseg-2024" // Fixed slug
                },
                 new Event
                 {
                     Id = 4,
                     Title = "Nádasdladányi Falunap 2025",
                     Description = "...",
                     StartDate = new DateTime(2026, 6, 9, 10, 0, 0, DateTimeKind.Utc), // Fixed
                     EndDate = new DateTime(2026, 6, 9, 23, 0, 0, DateTimeKind.Utc),   // Fixed
                     Location = "Központi Rendezvénytér",
                     Organizer = "Nádasdladány Önkormányzata",
                     EventUrl = null,
                     IsAllDay = false,
                     IsPublished = true,
                     Slug = "nadasdladanyi-falunap-2026" // Fixed slug
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
                    Description = "...",
                    FilePath = "documents/rendeletek/2024/1-2024-koltsegvetes.pdf",
                    FileType = "PDF",
                    FileSizeInBytes = 123456,
                    UploadedDate = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc), // Fixed
                    IsPublished = true,
                    DocumentCategoryId = catRendeletek.Id,
                    LastModifiedDate = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc) // Fixed
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
                    ImageUrl = "/img/mayor-placeholder.png", // Placeholder
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

            modelBuilder.Entity<GalleryAlbum>(album => {
                album.HasIndex(a => a.Slug).IsUnique();
                album.HasData(
                    new GalleryAlbum
                    {
                        Id = 1,
                        Title = "Falunapok",
                        Slug = "falunapok",
                        Description = "...",
                        CreatedDate = new DateTime(2023, 1, 10, 0, 0, 0, DateTimeKind.Utc), // WAS DYNAMIC, NOW FIXED
                        IsPublished = true,
                        DisplayOrder = 10
                    },
                    new GalleryAlbum
                    {
                        Id = 2,
                        Title = "Településképek",
                        Slug = "telepuleskepek",
                        Description = "...",
                        CreatedDate = new DateTime(2023, 1, 15, 0, 0, 0, DateTimeKind.Utc), // WAS DYNAMIC, NOW FIXED
                        IsPublished = true,
                        DisplayOrder = 20
                    },
                    new GalleryAlbum
                    {
                        Id = 3,
                        Title = "Nádasdy-kastély",
                        Slug = "nadasdy-kastely",
                        Description = "...",
                        CreatedDate = new DateTime(2023, 1, 20, 0, 0, 0, DateTimeKind.Utc), // WAS DYNAMIC, NOW FIXED
                        IsPublished = true,
                        DisplayOrder = 5
                    }
                );
            });

            // --- GalleryImage Configuration & Seed ---
            modelBuilder.Entity<GalleryImage>(image => {
                // Define the relationship to GalleryAlbum
                image.HasOne(i => i.GalleryAlbum)
                     .WithMany(a => a.Images)
                     .HasForeignKey(i => i.GalleryAlbumId)
                     .IsRequired(false) // Makes GalleryAlbumId nullable - an image doesn't HAVE to be in an album
                     .OnDelete(DeleteBehavior.SetNull); // If an album is deleted, set GalleryAlbumId to null for its images

                image.HasData(
                    new GalleryImage
                    {
                        Id = 1,
                        Title = "Falunapi Forgatag 2023",
                        Description = "...",
                        ImageUrl = "/images/gallery/falunap_2023_01.jpg",
                        ThumbnailUrl = "/images/gallery/thumbs/falunap_2023_01_thumb.jpg",
                        AltText = "Falunapi tömeg",
                        UploadedDate = fixedReferenceDateForGallery.AddMonths(11), // Based on a fixed reference
                        IsPublished = true,
                        DisplayOrder = 1,
                        GalleryAlbumId = 1
                    },
                    new GalleryImage
                    {
                        Id = 2,
                        Title = "Kastélypark Ősszel",
                        Description = "...",
                        ImageUrl = "/images/gallery/kastely_park_osz.jpg",
                        ThumbnailUrl = "/images/gallery/thumbs/kastely_park_osz_thumb.jpg",
                        AltText = "Nádasdy kastélypark ősszel",
                        UploadedDate = fixedReferenceDateForGallery.AddMonths(9), // Based on a fixed reference
                        IsPublished = true,
                        DisplayOrder = 1,
                        GalleryAlbumId = 3
                    },
                    new GalleryImage
                    {
                        Id = 3,
                        Title = "Főtér Naplementében",
                        Description = "Látkép a község főteréről naplementekor.",
                        ImageUrl = "/images/gallery/foter_naplemente.jpg",
                        ThumbnailUrl = "/images/gallery/thumbs/foter_naplemente_thumb.jpg",
                        AltText = "Főtér naplementében",
                        UploadedDate = fixedReferenceDateForGallery.AddMonths(7),
                        IsPublished = true,
                        DisplayOrder = 1,
                        GalleryAlbumId = 2 // Belongs to "Településképek"
                    },
                    new GalleryImage
                    {
                        Id = 4,
                        Title = "Gyerekek a Falunapon",
                        Description = "Ugrálóvár és vidámság.",
                        ImageUrl = "/images/gallery/falunap_gyerekek.jpg",
                        ThumbnailUrl = "/images/gallery/thumbs/falunap_gyerekek_thumb.jpg",
                        AltText = "Gyerekek az ugrálóvárban",
                        UploadedDate = fixedReferenceDateForGallery.AddMonths(11),
                        IsPublished = true,
                        DisplayOrder = 2,
                        GalleryAlbumId = 1
                    },
                    new GalleryImage
                    {
                        Id = 5,
                        Title = "Kastély Homlokzat",
                        Description = "A Nádasdy-kastély impozáns főhomlokzata.",
                        ImageUrl = "/images/gallery/kastely_homlokzat.jpg",
                        ThumbnailUrl = "/images/gallery/thumbs/kastely_homlokzat_thumb.jpg",
                        AltText = "Nádasdy-kastély homlokzat",
                        UploadedDate = fixedReferenceDateForGallery.AddMonths(5),
                        IsPublished = true,
                        DisplayOrder = 2,
                        GalleryAlbumId = 3
                    }
                );
            });

            modelBuilder.Entity<UsefulLink>().HasData(
                new UsefulLink
                {
                    Id = 1,
                    Title = "Magyarország.hu - Kormányzati Portál",
                    Url = "https://www.magyarorszag.hu",
                    Description = "Központi elektronikus ügyintézési és információs portál.",
                    OpenInNewTab = true,
                    Category = "Kormányzati",
                    IsPublished = true,
                    DisplayOrder = 10
                },
                new UsefulLink
                {
                    Id = 2,
                    Title = "Nemzeti Adó- és Vámhivatal (NAV)",
                    Url = "https://www.nav.gov.hu",
                    Description = "Adóügyekkel kapcsolatos információk és online ügyintézés.",
                    OpenInNewTab = true,
                    Category = "Kormányzati",
                    IsPublished = true,
                    DisplayOrder = 20
                },
                new UsefulLink
                {
                    Id = 3,
                    Title = "Fejér Vármegyei Kormányhivatal",
                    Url = "http://www.kormanyhivatal.hu/hu/fejer", // Verify exact URL
                    Description = "A vármegyei kormányhivatal hivatalos oldala.",
                    OpenInNewTab = true,
                    Category = "Kormányzati",
                    IsPublished = true,
                    DisplayOrder = 30
                },
                new UsefulLink
                {
                    Id = 4,
                    Title = "Helyi Hulladékszállítási Információk (VERTIKÁL)",
                    Url = "https://www.vertikalzrt.hu/", // Example, replace with actual provider if different
                    Description = "Hulladéknaptár és információk a szelektív hulladékgyűjtésről.",
                    OpenInNewTab = true,
                    Category = "Helyi Szolgáltatások",
                    IsPublished = true,
                    DisplayOrder = 40
                },
                new UsefulLink
                {
                    Id = 5,
                    Title = "MÁV-START (Vasúti Menetrend)",
                    Url = "https://www.mavcsoport.hu/mav-start/belfoldi-utazas/menetrend",
                    OpenInNewTab = true,
                    Category = "Közlekedés",
                    IsPublished = true,
                    DisplayOrder = 50
                },
                new UsefulLink
                {
                    Id = 6,
                    Title = "Volánbusz (Autóbusz Menetrend)",
                    Url = "https://www.volanbusz.hu/hu/menetrendek",
                    OpenInNewTab = true,
                    Category = "Közlekedés",
                    IsPublished = true,
                    DisplayOrder = 60
                }
            );
        }
    }
}
