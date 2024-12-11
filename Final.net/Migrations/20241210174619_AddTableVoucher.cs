using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.net.Migrations
{
    /// <inheritdoc />
    public partial class AddTableVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VoucherCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountPrice = table.Column<double>(type: "float", nullable: false),
                    MinPrice = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 9, 16, 29, 37, 887, DateTimeKind.Local).AddTicks(5533));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 9, 16, 29, 37, 887, DateTimeKind.Local).AddTicks(5566));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 9, 16, 29, 37, 887, DateTimeKind.Local).AddTicks(5567));
        }
    }
}
