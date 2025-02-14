using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2B_API.Migrations
{
    /// <inheritdoc />
    public partial class UserMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KPP",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "INN",
                table: "Users",
                type: "TEXT",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 12);

            migrationBuilder.AddColumn<string>(
                name: "OKPO",
                table: "Users",
                type: "TEXT",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UNP",
                table: "Users",
                type: "TEXT",
                maxLength: 9,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OKPO",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UNP",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "INN",
                table: "Users",
                type: "TEXT",
                maxLength: 12,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KPP",
                table: "Users",
                type: "TEXT",
                maxLength: 9,
                nullable: true);
        }
    }
}
