using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseExplorer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Budget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "budget_groups",
                schema: "expense",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget_groups", x => x.id);
                    table.CheckConstraint("ck_budget_groups_name_not_blank", "btrim(name) <> ''");
                });

            migrationBuilder.CreateTable(
                name: "budget_periods",
                schema: "expense",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget_periods", x => x.id);
                    table.CheckConstraint("ck_budget_periods_end_after_start", "end_date >= start_date");
                });

            migrationBuilder.CreateTable(
                name: "budget_categories",
                schema: "expense",
                columns: table => new
                {
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget_categories", x => x.category);
                    table.ForeignKey(
                        name: "FK_budget_categories_budget_groups_group_id",
                        column: x => x.group_id,
                        principalSchema: "expense",
                        principalTable: "budget_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "budget_funds",
                schema: "expense",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_id = table.Column<Guid>(type: "uuid", nullable: true),
                    position = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    day = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget_funds", x => x.id);
                    table.CheckConstraint("ck_budget_funds_day", "day between 1 and 31");
                    table.CheckConstraint("ck_budget_funds_name_not_blank", "btrim(name) <> ''");
                    table.ForeignKey(
                        name: "FK_budget_funds_budget_periods_period_id",
                        column: x => x.period_id,
                        principalSchema: "expense",
                        principalTable: "budget_periods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "budget_items",
                schema: "expense",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_id = table.Column<Guid>(type: "uuid", nullable: true),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    estimate = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    day = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget_items", x => x.id);
                    table.CheckConstraint("ck_budget_items_amount_not_negative", "amount >= 0 and (estimate is null or estimate >= 0)");
                    table.CheckConstraint("ck_budget_items_day", "day between 1 and 31");
                    table.CheckConstraint("ck_budget_items_name_not_blank", "btrim(name) <> ''");
                    table.ForeignKey(
                        name: "FK_budget_items_budget_groups_group_id",
                        column: x => x.group_id,
                        principalSchema: "expense",
                        principalTable: "budget_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_budget_items_budget_periods_period_id",
                        column: x => x.period_id,
                        principalSchema: "expense",
                        principalTable: "budget_periods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_budget_categories_group_id",
                schema: "expense",
                table: "budget_categories",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_funds_period_id",
                schema: "expense",
                table: "budget_funds",
                column: "period_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_groups_name",
                schema: "expense",
                table: "budget_groups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_budget_items_group_id",
                schema: "expense",
                table: "budget_items",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_items_period_id",
                schema: "expense",
                table: "budget_items",
                column: "period_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_periods_start_date",
                schema: "expense",
                table: "budget_periods",
                column: "start_date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "budget_categories",
                schema: "expense");

            migrationBuilder.DropTable(
                name: "budget_funds",
                schema: "expense");

            migrationBuilder.DropTable(
                name: "budget_items",
                schema: "expense");

            migrationBuilder.DropTable(
                name: "budget_groups",
                schema: "expense");

            migrationBuilder.DropTable(
                name: "budget_periods",
                schema: "expense");
        }
    }
}
