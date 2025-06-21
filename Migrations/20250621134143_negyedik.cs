using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nadasdladany.Migrations
{
    /// <inheritdoc />
    public partial class negyedik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "ContactInfo", "Description", "EndDate", "EventUrl", "IsAllDay", "IsPublished", "Location", "Organizer", "Slug", "StartDate", "Title" },
                values: new object[] { 4, null, "Nagyszabású falunap sok meglepetéssel, hagyományőrző programokkal, esti koncerttel és tűzijátékkal ünnepeljük községünket!", new DateTime(2026, 6, 9, 23, 0, 0, 0, DateTimeKind.Utc), null, false, true, "Központi Rendezvénytér", "Nádasdladány Önkormányzata", "nadasdladanyi-falunap-2026", new DateTime(2026, 6, 9, 10, 0, 0, 0, DateTimeKind.Utc), "Nádasdladányi Falunap 2026" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
