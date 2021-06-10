using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BankAccountAPI.Migrations
{
    public partial class AddIndexAndSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Address", "CreatedDate", "FirstName", "LastName", "ModifiedDate", "PostCode", "State" },
                values: new object[] { "Admin", "Addr", new DateTime(2021, 6, 10, 9, 32, 54, 91, DateTimeKind.Utc).AddTicks(1909), "First", "Last", new DateTime(2021, 6, 10, 9, 32, 54, 91, DateTimeKind.Utc).AddTicks(2451), 2000, "NSW" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountNumber",
                table: "Accounts",
                column: "AccountNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Accounts_AccountNumber",
                table: "Accounts");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "Admin");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
