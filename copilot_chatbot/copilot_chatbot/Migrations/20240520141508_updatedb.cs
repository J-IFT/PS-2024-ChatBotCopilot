using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace copilot_chatbot.Migrations
{
    public partial class updatedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imports_Products_ProductId",
                table: "Imports");

            migrationBuilder.RenameColumn(
                name: "GeneratedData",
                table: "Products",
                newName: "Species");

            migrationBuilder.RenameColumn(
                name: "Characteristics",
                table: "Products",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Blooming_season",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exposition",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Imports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Exports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsProcessed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Exported_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedDataProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedDataProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedDataProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExportGeneratedDataProduct",
                columns: table => new
                {
                    ExportsId = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneratedDataProductsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportGeneratedDataProduct", x => new { x.ExportsId, x.GeneratedDataProductsId });
                    table.ForeignKey(
                        name: "FK_ExportGeneratedDataProduct_Exports_ExportsId",
                        column: x => x.ExportsId,
                        principalTable: "Exports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExportGeneratedDataProduct_GeneratedDataProducts_GeneratedDataProductsId",
                        column: x => x.GeneratedDataProductsId,
                        principalTable: "GeneratedDataProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductKeywords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GeneratedDataProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    KeywordId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductKeywords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductKeywords_GeneratedDataProducts_GeneratedDataProductId",
                        column: x => x.GeneratedDataProductId,
                        principalTable: "GeneratedDataProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductKeywords_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExportGeneratedDataProduct_GeneratedDataProductsId",
                table: "ExportGeneratedDataProduct",
                column: "GeneratedDataProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_Exports_UserId",
                table: "Exports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedDataProducts_ProductId",
                table: "GeneratedDataProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductKeywords_GeneratedDataProductId_KeywordId",
                table: "ProductKeywords",
                columns: new[] { "GeneratedDataProductId", "KeywordId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductKeywords_KeywordId",
                table: "ProductKeywords",
                column: "KeywordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Imports_Products_ProductId",
                table: "Imports",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imports_Products_ProductId",
                table: "Imports");

            migrationBuilder.DropTable(
                name: "ExportGeneratedDataProduct");

            migrationBuilder.DropTable(
                name: "ProductKeywords");

            migrationBuilder.DropTable(
                name: "Exports");

            migrationBuilder.DropTable(
                name: "GeneratedDataProducts");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropColumn(
                name: "Blooming_season",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Exposition",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Species",
                table: "Products",
                newName: "GeneratedData");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Products",
                newName: "Characteristics");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Imports",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Imports_Products_ProductId",
                table: "Imports",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
