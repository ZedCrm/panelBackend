using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class atrebuteconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Id",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Persons_Id",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_CountTypes_Id",
                table: "CountTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_Id",
                table: "Products",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Id",
                table: "Persons",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CountTypes_Id",
                table: "CountTypes",
                column: "Id");
        }
    }
}
