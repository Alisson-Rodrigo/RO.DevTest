using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RO.DevTest.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlterCollumsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataVenda",
                table: "Sales",
                newName: "DateSale");

            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "SaleItems",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "PrecoUnitario",
                table: "SaleItems",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "CartItems",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "PrecoUnitario",
                table: "CartItems",
                newName: "UnitPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateSale",
                table: "Sales",
                newName: "DataVenda");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "SaleItems",
                newName: "PrecoUnitario");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "SaleItems",
                newName: "Quantidade");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "CartItems",
                newName: "PrecoUnitario");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "CartItems",
                newName: "Quantidade");
        }
    }
}
