using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nadasdladany.Migrations
{
    /// <inheritdoc />
    public partial class masodik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Events",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsPublished", "Slug" },
                values: new object[] { true, "nadasdladanyi-falunap-2024" });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsPublished", "Slug" },
                values: new object[] { true, "konyvtari-olvasoklub-nyari-olvasmanyok" });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsPublished", "Slug" },
                values: new object[] { true, "idosek-napja-unnepseg-2024" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Events");
        }
    }
}
