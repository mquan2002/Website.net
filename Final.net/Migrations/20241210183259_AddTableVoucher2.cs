using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.net.Migrations
{
    /// <inheritdoc />
    public partial class AddTableVoucher2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 11, 1, 32, 58, 104, DateTimeKind.Local).AddTicks(4443));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 11, 1, 32, 58, 104, DateTimeKind.Local).AddTicks(4457));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 11, 1, 32, 58, 104, DateTimeKind.Local).AddTicks(4458));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 11, 0, 46, 17, 630, DateTimeKind.Local).AddTicks(8481));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 11, 0, 46, 17, 630, DateTimeKind.Local).AddTicks(8495));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 11, 0, 46, 17, 630, DateTimeKind.Local).AddTicks(8497));
        }
    }
}
