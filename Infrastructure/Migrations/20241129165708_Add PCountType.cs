using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPCountType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountType",
                table: "Products");

            migrationBuilder.AddColumn<short>(
                name: "CountTypeId",
                table: "Products",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "PCountType",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCountType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CountTypeId",
                table: "Products",
                column: "CountTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_PCountType_CountTypeId",
                table: "Products",
                column: "CountTypeId",
                principalTable: "PCountType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_PCountType_CountTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "PCountType");

            migrationBuilder.DropIndex(
                name: "IX_Products_CountTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CountTypeId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "CountType",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
