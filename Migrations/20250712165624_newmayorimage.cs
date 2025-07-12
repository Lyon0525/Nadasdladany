using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nadasdladany.Migrations
{
    /// <inheritdoc />
    public partial class newmayorimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Representatives",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/img/mayor-placeholder.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Representatives",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/representatives/mayor-varga-tunde.jpg");
        }
    }
}
