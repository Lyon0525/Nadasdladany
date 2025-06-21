using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nadasdladany.Migrations
{
    /// <inheritdoc />
    public partial class dbmunicipality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfficeHourEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    TimeDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeHourEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfficeInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AboutOffice = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GoogleMapsEmbedUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Representatives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CustomTitleOverride = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ReceptionHoursInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Biography = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Representatives", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "OfficeHourEntries",
                columns: new[] { "Id", "DayOfWeek", "DisplayOrder", "TimeDescription" },
                values: new object[,]
                {
                    { 1, 1, 1, "8:00 - 12:00 és 13:00 - 16:00" },
                    { 2, 2, 2, "Nincs ügyfélfogadás" },
                    { 3, 3, 3, "8:00 - 12:00 és 13:00 - 17:00 (Hosszított)" },
                    { 4, 4, 4, "Nincs ügyfélfogadás" },
                    { 5, 5, 5, "8:00 - 12:00" },
                    { 6, 6, 6, "Zárva" },
                    { 7, 0, 7, "Zárva" }
                });

            migrationBuilder.InsertData(
                table: "OfficeInfos",
                columns: new[] { "Id", "AboutOffice", "Address", "Email", "GoogleMapsEmbedUrl", "OfficeName", "PhoneNumber", "WebsiteUrl" },
                values: new object[] { 1, "<p>A Nádasdladányi Polgármesteri Hivatal felelős a helyi önkormányzati feladatok ellátásáért, a képviselő-testület döntéseinek végrehajtásáért, valamint a lakossági ügyintézésért. Célunk a hatékony, átlátható és ügyfélbarát közigazgatás biztosítása.</p><p>Munkatársaink készséggel állnak rendelkezésükre a különböző ügytípusokban, legyen szó adóügyekről, szociális támogatásokról, anyakönyvi kivonatokról vagy helyi engedélyekről.</p>", "8145 Nádasdladány, Fő utca 1.", "hivatal@nadasdladany.hu", "<iframe src=\"https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d2710.894867318367!2d18.23535761561613!3d47.20190097915973!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x4769eb3b71697cc9%3A0x3a905520d403d2b9!2sN%C3%A1dasdlad%C3%A1ny%2C%20F%C5%91%20utca%201!5e0!3m2!1shu!2shu!4v1620000000000!5m2!1shu!2shu\" width=\"600\" height=\"450\" style=\"border:0;\" allowfullscreen=\"\" loading=\"lazy\"></iframe>", "Nádasdladányi Polgármesteri Hivatal", "+36 (22) 123-456", "https://www.nadasdladany.hu" });

            migrationBuilder.InsertData(
                table: "Representatives",
                columns: new[] { "Id", "Biography", "CustomTitleOverride", "DisplayOrder", "Email", "ImageUrl", "IsPublished", "Name", "PhoneNumber", "ReceptionHoursInfo", "Role" },
                values: new object[,]
                {
                    { 1, "<p>Tisztelt Nádasdladányiak! Polgármesterként legfőbb célom községünk fejlődése és közösségünk erősítése. Nyitott ajtókkal várom Önöket!</p>", null, 1, "polgarmester@nadasdladany.hu", "/images/representatives/mayor-varga-tunde.jpg", true, "Varga Tünde", "+36 (30) 111-2233", "Minden hónap első szerdáján 14:00-16:00 (Előzetes bejelentkezés szükséges)", 0 },
                    { 2, null, "Alpolgármester, Pénzügyi és Ügyrendi Bizottság Elnöke", 2, "alpolgarmester@nadasdladany.hu", "/images/representatives/kovacs-istvan.jpg", true, "Dr. Kovács István", null, null, 1 },
                    { 3, null, "Képviselő, Szociális és Kulturális Bizottság Tagja", 3, null, "/images/representatives/nagy-maria.jpg", true, "Nagy Mária", null, null, 2 },
                    { 4, null, "Jegyző", 10, "jegyzo@nadasdladany.hu", "/images/representatives/horvath-geza-jegyzo.jpg", true, "Horváth Géza", "+36 (22) 123-457", null, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfficeHourEntries");

            migrationBuilder.DropTable(
                name: "OfficeInfos");

            migrationBuilder.DropTable(
                name: "Representatives");
        }
    }
}
