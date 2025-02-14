using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace B2B_API.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashColumnToUsersAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    StockQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    SKU = table.Column<string>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: true),
                    Unit = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    UserType = table.Column<int>(type: "INTEGER", nullable: false),
                    UserRole = table.Column<int>(type: "INTEGER", nullable: false),
                    INN = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    KPP = table.Column<string>(type: "TEXT", maxLength: 9, nullable: true),
                    LegalAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ActualAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    BankName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    BankAccount = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    BankBIK = table.Column<string>(type: "TEXT", maxLength: 9, nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SellerId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceLists_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PriceLists_Users_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceListProducts",
                columns: table => new
                {
                    PriceListId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    SpecialPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceListProducts", x => new { x.PriceListId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_PriceListProducts_PriceLists_PriceListId",
                        column: x => x.PriceListId,
                        principalTable: "PriceLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceListProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceListUser",
                columns: table => new
                {
                    AccessiblePriceListsId = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowedBuyersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceListUser", x => new { x.AccessiblePriceListsId, x.AllowedBuyersId });
                    table.ForeignKey(
                        name: "FK_PriceListUser_PriceLists_AccessiblePriceListsId",
                        column: x => x.AccessiblePriceListsId,
                        principalTable: "PriceLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceListUser_Users_AllowedBuyersId",
                        column: x => x.AllowedBuyersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceListProducts_ProductId",
                table: "PriceListProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceLists_ProductId",
                table: "PriceLists",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceLists_SellerId",
                table: "PriceLists",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListUser_AllowedBuyersId",
                table: "PriceListUser",
                column: "AllowedBuyersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceListProducts");

            migrationBuilder.DropTable(
                name: "PriceListUser");

            migrationBuilder.DropTable(
                name: "PriceLists");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
