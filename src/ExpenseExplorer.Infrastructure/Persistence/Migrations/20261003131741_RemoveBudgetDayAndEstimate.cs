using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseExplorer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBudgetDayAndEstimate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "day",
                schema: "expense",
                table: "budget_items");

            migrationBuilder.DropColumn(
                name: "estimate",
                schema: "expense",
                table: "budget_items");

            migrationBuilder.DropColumn(
                name: "day",
                schema: "expense",
                table: "budget_funds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "day",
                schema: "expense",
                table: "budget_items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "estimate",
                schema: "expense",
                table: "budget_items",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "day",
                schema: "expense",
                table: "budget_funds",
                type: "integer",
                nullable: true);
        }
    }
}
