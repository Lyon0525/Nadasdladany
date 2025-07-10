using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nadasdladany.Migrations
{
    /// <inheritdoc />
    public partial class links : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsefulLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OpenInNewTab = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsefulLinks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UsefulLinks",
                columns: new[] { "Id", "Category", "Description", "DisplayOrder", "IsPublished", "OpenInNewTab", "Title", "Url" },
                values: new object[,]
                {
                    { 1, "Kormányzati", "Központi elektronikus ügyintézési és információs portál.", 10, true, true, "Magyarország.hu - Kormányzati Portál", "https://www.magyarorszag.hu" },
                    { 2, "Kormányzati", "Adóügyekkel kapcsolatos információk és online ügyintézés.", 20, true, true, "Nemzeti Adó- és Vámhivatal (NAV)", "https://www.nav.gov.hu" },
                    { 3, "Kormányzati", "A vármegyei kormányhivatal hivatalos oldala.", 30, true, true, "Fejér Vármegyei Kormányhivatal", "http://www.kormanyhivatal.hu/hu/fejer" },
                    { 4, "Helyi Szolgáltatások", "Hulladéknaptár és információk a szelektív hulladékgyűjtésről.", 40, true, true, "Helyi Hulladékszállítási Információk (VERTIKÁL)", "https://www.vertikalzrt.hu/" },
                    { 5, "Közlekedés", null, 50, true, true, "MÁV-START (Vasúti Menetrend)", "https://www.mavcsoport.hu/mav-start/belfoldi-utazas/menetrend" },
                    { 6, "Közlekedés", null, 60, true, true, "Volánbusz (Autóbusz Menetrend)", "https://www.volanbusz.hu/hu/menetrendek" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsefulLinks");
        }
    }
}
