using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bluewave.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesAndConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "products",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_tax_id",
                table: "suppliers",
                column: "tax_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_suppliers_tax_id",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "products");
        }
    }
}
