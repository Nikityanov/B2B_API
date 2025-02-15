using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2B_API.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceListDescriptionTypeCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "PriceLists",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PriceLists",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceListType",
                table: "PriceLists",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "PriceLists");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PriceLists");

            migrationBuilder.DropColumn(
                name: "PriceListType",
                table: "PriceLists");
        }
    }
}
