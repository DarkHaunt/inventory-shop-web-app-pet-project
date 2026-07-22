using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsEquipped = table.Column<bool>(type: "boolean", nullable: false),
                    IsOnSale = table.Column<bool>(type: "boolean", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    StatsModifiers_Agility = table.Column<int>(type: "integer", nullable: false),
                    StatsModifiers_Intelligence = table.Column<int>(type: "integer", nullable: false),
                    StatsModifiers_Strength = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: false),
                    PasswordHashed = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LevelProgress_Level = table.Column<long>(type: "bigint", nullable: false),
                    LevelProgress_Experience = table.Column<long>(type: "bigint", nullable: false),
                    Wallet_GoldAmount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderData_ItemSnapshot_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderData_ItemSnapshot_Type = table.Column<int>(type: "integer", nullable: false),
                    OrderData_ItemSnapshot_Description = table.Column<string>(type: "text", nullable: true),
                    OrderData_ItemSnapshot_StatsModifiers_Agility = table.Column<int>(type: "integer", nullable: false),
                    OrderData_ItemSnapshot_StatsModifiers_Strength = table.Column<int>(type: "integer", nullable: false),
                    OrderData_ItemSnapshot_StatsModifiers_Intelligence = table.Column<int>(type: "integer", nullable: false),
                    OrderData_ItemSnapshot_CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderData_Price_GoldAmount = table.Column<long>(type: "bigint", nullable: false),
                    OrderData_RequiredLevel = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SellItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequiredLevel_Level = table.Column<long>(type: "bigint", nullable: false),
                    RequiredLevel_Experience = table.Column<long>(type: "bigint", nullable: false),
                    Price_GoldAmount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopSlots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_Nickname",
                table: "Players",
                column: "Nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_PasswordHashed",
                table: "Players",
                column: "PasswordHashed",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "ShopOrders");

            migrationBuilder.DropTable(
                name: "ShopSlots");
        }
    }
}
