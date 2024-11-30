using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.net.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 11, 29, 10, 23, 55, 261, DateTimeKind.Local).AddTicks(966));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 11, 29, 10, 23, 55, 261, DateTimeKind.Local).AddTicks(983));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedDate", "IsDeleted", "Name", "UpdatedDate" },
                values: new object[] { 3, new DateTime(2024, 11, 29, 10, 23, 55, 261, DateTimeKind.Local).AddTicks(984), false, "Staff", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 11, 17, 13, 8, 12, 753, DateTimeKind.Local).AddTicks(6977));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 11, 17, 13, 8, 12, 753, DateTimeKind.Local).AddTicks(6994));
        }
    }
}
