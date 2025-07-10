using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nadasdladany.Migrations
{
    /// <inheritdoc />
    public partial class gallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GalleryAlbums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryAlbums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GalleryImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AltText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    GalleryAlbumId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GalleryImages_GalleryAlbums_GalleryAlbumId",
                        column: x => x.GalleryAlbumId,
                        principalTable: "GalleryAlbums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Content", "Excerpt" },
                values: new object[] { "<p>...</p>", "..." });

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Content", "Excerpt" },
                values: new object[] { "<p>...</p>", "..." });

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Content", "Excerpt" },
                values: new object[] { "<p>...</p>", "..." });

            migrationBuilder.UpdateData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "...");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "...");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Slug", "Title" },
                values: new object[] { "...", "konyvtari-olvasoklub-nyari-olvasmanyok-2024", "Könyvtári Olvasóklub: Nyári Olvasmányok 2024" });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Title" },
                values: new object[] { "...", "Idősek Napja Ünnepség 2024" });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Title" },
                values: new object[] { "...", "Nádasdladányi Falunap 2025" });

            migrationBuilder.InsertData(
                table: "GalleryAlbums",
                columns: new[] { "Id", "CreatedDate", "Description", "DisplayOrder", "IsPublished", "Slug", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "...", 10, true, "falunapok", "Falunapok" },
                    { 2, new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "...", 20, true, "telepuleskepek", "Településképek" },
                    { 3, new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "...", 5, true, "nadasdy-kastely", "Nádasdy-kastély" }
                });

            migrationBuilder.InsertData(
                table: "GalleryImages",
                columns: new[] { "Id", "AltText", "Description", "DisplayOrder", "GalleryAlbumId", "ImageUrl", "IsPublished", "ThumbnailUrl", "Title", "UploadedDate" },
                values: new object[,]
                {
                    { 1, "Falunapi tömeg", "...", 1, 1, "/images/gallery/falunap_2023_01.jpg", true, "/images/gallery/thumbs/falunap_2023_01_thumb.jpg", "Falunapi Forgatag 2023", new DateTime(2023, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Nádasdy kastélypark ősszel", "...", 1, 3, "/images/gallery/kastely_park_osz.jpg", true, "/images/gallery/thumbs/kastely_park_osz_thumb.jpg", "Kastélypark Ősszel", new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "Főtér naplementében", "Látkép a község főteréről naplementekor.", 1, 2, "/images/gallery/foter_naplemente.jpg", true, "/images/gallery/thumbs/foter_naplemente_thumb.jpg", "Főtér Naplementében", new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "Gyerekek az ugrálóvárban", "Ugrálóvár és vidámság.", 2, 1, "/images/gallery/falunap_gyerekek.jpg", true, "/images/gallery/thumbs/falunap_gyerekek_thumb.jpg", "Gyerekek a Falunapon", new DateTime(2023, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "Nádasdy-kastély homlokzat", "A Nádasdy-kastély impozáns főhomlokzata.", 2, 3, "/images/gallery/kastely_homlokzat.jpg", true, "/images/gallery/thumbs/kastely_homlokzat_thumb.jpg", "Kastély Homlokzat", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GalleryAlbums_Slug",
                table: "GalleryAlbums",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImages_GalleryAlbumId",
                table: "GalleryImages",
                column: "GalleryAlbumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GalleryImages");

            migrationBuilder.DropTable(
                name: "GalleryAlbums");

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Content", "Excerpt" },
                values: new object[] { "<p>Részletes leírás az ünnepi megemlékezésről, amely a nemzeti ünnepünk alkalmából került megrendezésre a község főterén. Beszédet mondott Varga Tünde polgármester asszony.</p>", "Rövid összefoglaló az ünnepi megemlékezésről, amely a főtéren került megrendezésre." });

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Content", "Excerpt" },
                values: new object[] { "<p>Az új játszótér átadásának részletei. Modern játékokkal és biztonságos környezettel várjuk a gyermekeket és családjaikat.</p>", "Örömmel jelentjük be, hogy átadásra került a felújított központi játszótér a Kossuth utcában." });

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Content", "Excerpt" },
                values: new object[] { "<p>Tájékoztatjuk a tisztelt lakosságot, hogy a következő közmeghallgatás időpontja 2024. május 15., 17:00. Helyszín: Művelődési Ház. Témák: éves költségvetés, fejlesztési tervek.</p>", "Fontos információk a következő közmeghallgatásról, melynek fő témái a költségvetés és a fejlesztések lesznek." });

            migrationBuilder.UpdateData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Nádasdladány község 2024. évi költségvetéséről szóló 1/2024. (II.15.) önkormányzati rendelet.");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Egész napos programok kicsiknek és nagyoknak. Koncertek, főzőverseny, kézműves vásár, gyerekprogramok és esti tűzijáték.");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Slug", "Title" },
                values: new object[] { "Beszélgetés a nyár legnépszerűbb könyveiről, ajánlók és közös élmények megosztása.", "konyvtari-olvasoklub-nyari-olvasmanyok", "Könyvtári Olvasóklub: Nyári Olvasmányok" });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Szeretettel várjuk szépkorú lakosainkat egy közös ünnepségre a Művelődési Ház nagytermében. Ünnepi műsorral és vendéglátással készülünk.", "Idősek Napja Ünnepség" });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Nagyszabású falunap sok meglepetéssel, hagyományőrző programokkal, esti koncerttel és tűzijátékkal ünnepeljük községünket!", "Nádasdladányi Falunap 2026" });
        }
    }
}
