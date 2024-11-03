using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Final.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CodeName", "CreatedAt", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "ADMIN", new DateTime(2024, 11, 3, 23, 52, 26, 719, DateTimeKind.Local).AddTicks(6759), false, "Admin", new DateTime(2024, 11, 3, 23, 52, 26, 719, DateTimeKind.Local).AddTicks(6770) },
                    { 2, "USER", new DateTime(2024, 11, 3, 23, 52, 26, 719, DateTimeKind.Local).AddTicks(6772), false, "User", new DateTime(2024, 11, 3, 23, 52, 26, 719, DateTimeKind.Local).AddTicks(6772) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
