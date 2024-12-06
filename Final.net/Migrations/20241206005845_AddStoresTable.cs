using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.net.Migrations
{
    /// <inheritdoc />
    public partial class AddStoresTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 6, 7, 58, 45, 291, DateTimeKind.Local).AddTicks(917));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 6, 7, 58, 45, 291, DateTimeKind.Local).AddTicks(964));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 6, 7, 58, 45, 291, DateTimeKind.Local).AddTicks(965));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 6, 7, 52, 36, 596, DateTimeKind.Local).AddTicks(359));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 6, 7, 52, 36, 596, DateTimeKind.Local).AddTicks(372));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 6, 7, 52, 36, 596, DateTimeKind.Local).AddTicks(373));
        }
    }
}
