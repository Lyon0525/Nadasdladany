using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nadasdladany.Migrations
{
    /// <inheritdoc />
    public partial class intezmenyek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactSubmissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Institutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IconCssClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Institutions",
                columns: new[] { "Id", "Address", "Description", "DisplayOrder", "Email", "IconCssClass", "ImageUrl", "IsPublished", "Name", "PhoneNumber", "Slug", "WebsiteUrl" },
                values: new object[,]
                {
                    { 1, "Nádasdladány, Iskola utca 1. (P)", "Községünk általános iskolája, amely alapfokú oktatást biztosít a helyi és környékbeli gyermekek számára.", 10, "iskola@nadasdladany.edu.hu (P)", "bi bi-book-fill", "/images/institutions/iskola-placeholder.jpg", true, "Nádasdy Ferenc Általános Iskola", "+36 (22) 234-567 (P)", "nadasdy-ferenc-altalanos-iskola", "http://www.nadasdyiskola-nl.hu/" },
                    { 2, "Nádasdladány, Óvoda köz 2. (P)", "Szeretetteljes és biztonságos környezetben neveljük a legkisebbeket, felkészítve őket az iskolás évekre.", 20, "ovoda@nadasdladany.edu.hu (P)", "bi bi-palette-fill", "/images/institutions/ovoda-placeholder.jpg", true, "Napraforgó Óvoda és Bölcsőde", "+36 (22) 345-678 (P)", "napraforgo-ovoda-es-bolcsode", null },
                    { 3, "Nádasdladány, Kultúra tér 3. (P)", "Közösségi programok, kulturális események, kiállítások és könyvtári szolgáltatások helyszíne.", 30, "muvhaz@nadasdladany.info (P)", "bi bi-collection-play-fill", "/images/institutions/muvhaz-placeholder.jpg", true, "Művelődési Ház és Könyvtár", "+36 (22) 456-789 (P)", "muvelodesi-haz-es-konyvtar", "http://www.nadasdladanyimuvelodesihaz.hu" },
                    { 4, "Nádasdladány, Egészség út 4. (P)", "Háziorvosi és gyermekorvosi ellátás a település lakosai számára.", 40, null, "bi bi-heart-pulse-fill", "/images/institutions/orvosi-rendelo-placeholder.jpg", true, "Orvosi Rendelő (Háziorvos)", "+36 (22) 567-890 (P)", "orvosi-rendelo-haziorvos", null },
                    { 5, "Nádasdladány, Egészség út 4/B. (P)", "Fogászati alapellátás és szakellátás.", 50, null, "bi bi-bandaid-fill", "/images/institutions/fogorvosi-rendelo-placeholder.jpg", true, "Fogorvosi Rendelő", "+36 (22) 678-901 (P)", "fogorvosi-rendelo", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_Slug",
                table: "Institutions",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactSubmissions");

            migrationBuilder.DropTable(
                name: "Institutions");
        }
    }
}
