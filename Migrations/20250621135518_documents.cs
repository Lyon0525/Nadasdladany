using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nadasdladany.Migrations
{
    /// <inheritdoc />
    public partial class documents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocumentCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentCategories_DocumentCategoryId",
                        column: x => x.DocumentCategoryId,
                        principalTable: "DocumentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DocumentCategories",
                columns: new[] { "Id", "Description", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, "Önkormányzati rendeletek gyűjteménye.", "Rendeletek", "rendeletek" },
                    { 2, "Képviselő-testületi és bizottsági ülések jegyzőkönyvei.", "Jegyzőkönyvek", "jegyzokonyvek" },
                    { 3, "A Képviselő-testület és a polgármester határozatai.", "Határozatok", "hatarozatok" },
                    { 4, "Törvény által kötelezően közzéteendő közérdekű adatok.", "Közérdekű Adatok", "kozerdeku-adatok" },
                    { 5, "Elnyert pályázatok és azokhoz kapcsolódó dokumentumok, beszámolók.", "Pályázatok és Beszámolók", "palyazatok-beszamolok" }
                });

            migrationBuilder.InsertData(
                table: "Documents",
                columns: new[] { "Id", "Description", "DocumentCategoryId", "FilePath", "FileSizeInBytes", "FileType", "IsPublished", "LastModifiedDate", "Title", "UploadedDate" },
                values: new object[,]
                {
                    { 1, "Nádasdladány község 2024. évi költségvetéséről szóló 1/2024. (II.15.) önkormányzati rendelet.", 1, "documents/rendeletek/2024/1-2024-koltsegvetes.pdf", 123456L, "PDF", true, new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "2024. évi költségvetési rendelet", new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "A Képviselő-testület rendes ülésének jegyzőkönyve.", 2, "documents/jegyzokonyvek/2024/2024-03-10-testuleti-ules.pdf", 234567L, "PDF", true, new DateTime(2024, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "2024. március 10-i testületi ülés jegyzőkönyve", new DateTime(2024, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "Az önkormányzat adatkezelési gyakorlatáról szóló hivatalos tájékoztató.", 4, "documents/kozerdeku/adatkezelesi-tajekoztato-2024.pdf", 98765L, "PDF", true, new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Adatkezelési Tájékoztató", new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentCategories_Slug",
                table: "DocumentCategories",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentCategoryId",
                table: "Documents",
                column: "DocumentCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentCategories");
        }
    }
}
