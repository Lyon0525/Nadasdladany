using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nadasdladany.Migrations
{
    /// <inheritdoc />
    public partial class elso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsAllDay = table.Column<bool>(type: "bit", nullable: false),
                    Organizer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactInfo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EventUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Excerpt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FeaturedImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, "Hírek a helyi önkormányzat működéséről.", "Önkormányzati Hírek", "onkormanyzati-hirek" },
                    { 2, "Információk a település közösségi programjairól.", "Közösségi Események", "kozossegi-esemenyek" },
                    { 3, "Fontos közlemények a lakosság számára.", "Közlemények", "kozlemenyek" }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "ContactInfo", "Description", "EndDate", "EventUrl", "IsAllDay", "Location", "Organizer", "StartDate", "Title" },
                values: new object[,]
                {
                    { 1, null, "Egész napos programok kicsiknek és nagyoknak. Koncertek, főzőverseny, kézműves vásár, gyerekprogramok és esti tűzijáték.", new DateTime(2024, 6, 16, 22, 0, 0, 0, DateTimeKind.Utc), "/esemenyek/falunap-2024", false, "Községi Sportpálya és Szabadidőpark", "Nádasdladány Önkormányzat", new DateTime(2024, 6, 16, 10, 0, 0, 0, DateTimeKind.Utc), "Nádasdladányi Falunap 2024" },
                    { 2, "konyvtar@nadasdladany.hu", "Beszélgetés a nyár legnépszerűbb könyveiről, ajánlók és közös élmények megosztása.", new DateTime(2024, 6, 26, 18, 0, 0, 0, DateTimeKind.Utc), null, false, "Helyi Könyvtár Olvasóterme", "Nádasdladányi Könyvtár", new DateTime(2024, 6, 26, 17, 0, 0, 0, DateTimeKind.Utc), "Könyvtári Olvasóklub: Nyári Olvasmányok" },
                    { 3, null, "Szeretettel várjuk szépkorú lakosainkat egy közös ünnepségre a Művelődési Ház nagytermében. Ünnepi műsorral és vendéglátással készülünk.", null, null, false, "Művelődési Ház", "Nádasdladány Önkormányzat és a Helyi Nyugdíjas Klub", new DateTime(2024, 7, 11, 15, 0, 0, 0, DateTimeKind.Utc), "Idősek Napja Ünnepség" }
                });

            migrationBuilder.InsertData(
                table: "Articles",
                columns: new[] { "Id", "Author", "CategoryId", "Content", "Excerpt", "FeaturedImageUrl", "IsPublished", "LastModifiedDate", "PublishedDate", "Slug", "Title", "ViewCount" },
                values: new object[,]
                {
                    { 1, "Nádasdladány Önkormányzat", 2, "<p>Részletes leírás az ünnepi megemlékezésről, amely a nemzeti ünnepünk alkalmából került megrendezésre a község főterén. Beszédet mondott Varga Tünde polgármester asszony.</p>", "Rövid összefoglaló az ünnepi megemlékezésről, amely a főtéren került megrendezésre.", "/images/placeholder/news_placeholder_1.jpg", true, new DateTime(2024, 3, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 3, 15, 10, 0, 0, 0, DateTimeKind.Utc), "unnepi-megemlekezes-foteren", "Ünnepi Megemlékezés a Főtéren", 0 },
                    { 2, "Nádasdladány Önkormányzat", 1, "<p>Az új játszótér átadásának részletei. Modern játékokkal és biztonságos környezettel várjuk a gyermekeket és családjaikat.</p>", "Örömmel jelentjük be, hogy átadásra került a felújított központi játszótér a Kossuth utcában.", "/images/placeholder/news_placeholder_2.jpg", true, new DateTime(2024, 4, 22, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 4, 22, 14, 0, 0, 0, DateTimeKind.Utc), "uj-jatszoter-atadasa-kossuth-utcaban", "Új Játszótér Átadása a Kossuth Utcában", 0 },
                    { 3, "Nádasdladány Önkormányzat", 3, "<p>Tájékoztatjuk a tisztelt lakosságot, hogy a következő közmeghallgatás időpontja 2024. május 15., 17:00. Helyszín: Művelődési Ház. Témák: éves költségvetés, fejlesztési tervek.</p>", "Fontos információk a következő közmeghallgatásról, melynek fő témái a költségvetés és a fejlesztések lesznek.", null, true, new DateTime(2024, 5, 1, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 5, 1, 9, 0, 0, 0, DateTimeKind.Utc), "kozmeghallgatas-idopontja-temai", "Közmeghallgatás Időpontja és Témái", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Slug",
                table: "Articles",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
