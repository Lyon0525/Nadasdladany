using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nadasdladany.Migrations
{
    /// <inheritdoc />
    public partial class editablehomemessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    SettingKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.SettingKey);
                });

            migrationBuilder.InsertData(
                table: "SiteSettings",
                columns: new[] { "SettingKey", "SettingValue" },
                values: new object[,]
                {
                    { "MayorName", "Varga Tünde" },
                    { "WelcomeMessageParagraph1", "Szeretettel köszöntöm Önt Nádasdladány község hivatalos honlapján! Célunk, hogy ezen a felületen keresztül átfogó képet adjunk településünk mindennapjairól, működéséről, valamint lehetőséget biztosítsunk az egyszerű és gyors tájékozódásra." },
                    { "WelcomeMessageParagraph2", "Böngésszen híreink között, ismerje meg önkormányzatunk munkáját, intézményeinket és fedezze fel Nádasdladány természeti és épített értékeit. Reméljük, hasznos információkkal szolgálhatunk minden kedves érdeklődő számára." },
                    { "WelcomeTitle", "Tisztelt Látogató!" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteSettings");
        }
    }
}
