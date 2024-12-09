using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.net.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationUserToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "CartItems",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrderItemId",
                table: "Users",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_OrderItemId",
                table: "CartItems",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_OrderItem_OrderItemId",
                table: "CartItems",
                column: "OrderItemId",
                principalTable: "OrderItem",
                principalColumn: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_OrderItem_OrderItemId",
                table: "Users",
                column: "OrderItemId",
                principalTable: "OrderItem",
                principalColumn: "OrderItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_OrderItem_OrderItemId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_OrderItem_OrderItemId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_OrderItemId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_OrderItemId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "CartItems");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 9, 13, 58, 3, 202, DateTimeKind.Local).AddTicks(4260));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 9, 13, 58, 3, 202, DateTimeKind.Local).AddTicks(4278));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 12, 9, 13, 58, 3, 202, DateTimeKind.Local).AddTicks(4279));
        }
    }
}
