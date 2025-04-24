using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcounttypegetandcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_PCountType_CountTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "PCountType");

            migrationBuilder.CreateTable(
                name: "CountTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CountTypes_Id",
                table: "CountTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CountTypes_CountTypeId",
                table: "Products",
                column: "CountTypeId",
                principalTable: "CountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CountTypes_CountTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "CountTypes");

            migrationBuilder.CreateTable(
                name: "PCountType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCountType", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_PCountType_CountTypeId",
                table: "Products",
                column: "CountTypeId",
                principalTable: "PCountType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
