using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DineReserve.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_dish_properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KiloCalories",
                table: "Dishes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Dishes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KiloCalories",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Dishes");
        }
    }
}
