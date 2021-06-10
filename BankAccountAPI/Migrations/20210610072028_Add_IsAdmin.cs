using Microsoft.EntityFrameworkCore.Migrations;

namespace BankAccountAPI.Migrations
{
    public partial class Add_IsAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_UserName",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Accounts",
                newName: "Username");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_UserName",
                table: "Accounts",
                newName: "IX_Accounts_Username");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_Username",
                table: "Accounts",
                column: "Username",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_Username",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Accounts",
                newName: "UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                newName: "IX_Accounts_UserName");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_UserName",
                table: "Accounts",
                column: "UserName",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
