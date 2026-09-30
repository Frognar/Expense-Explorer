using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseExplorer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "expense");

            migrationBuilder.CreateTable(
                name: "receipts",
                schema: "expense",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    store = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    purchase_date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipts", x => x.id);
                    table.CheckConstraint("ck_receipts_store_not_blank", "btrim(store) <> ''");
                });

            migrationBuilder.CreateTable(
                name: "receipt_items",
                schema: "expense",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    receipt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    item = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    discount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipt_items", x => x.id);
                    table.CheckConstraint("ck_receipt_items_amount_not_negative", "amount >= 0");
                    table.CheckConstraint("ck_receipt_items_category_not_blank", "btrim(category) <> ''");
                    table.CheckConstraint("ck_receipt_items_discount_within_amount", "discount >= 0 and discount <= amount");
                    table.CheckConstraint("ck_receipt_items_item_not_blank", "btrim(item) <> ''");
                    table.CheckConstraint("ck_receipt_items_quantity_positive", "quantity > 0");
                    table.ForeignKey(
                        name: "FK_receipt_items_receipts_receipt_id",
                        column: x => x.receipt_id,
                        principalSchema: "expense",
                        principalTable: "receipts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_receipt_items_category",
                schema: "expense",
                table: "receipt_items",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_items_item",
                schema: "expense",
                table: "receipt_items",
                column: "item");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_items_receipt_id",
                schema: "expense",
                table: "receipt_items",
                column: "receipt_id");

            migrationBuilder.CreateIndex(
                name: "IX_receipts_purchase_date",
                schema: "expense",
                table: "receipts",
                column: "purchase_date");

            migrationBuilder.CreateIndex(
                name: "IX_receipts_store",
                schema: "expense",
                table: "receipts",
                column: "store");

            // One-time copy of the data kept by the previous app version in the public schema.
            // Its lines stored a unit price; the amount for the whole line is that price times
            // the quantity, rounded half away from zero to grosze. The old tables stay as a backup.
            migrationBuilder.Sql("""
                do $$
                begin
                    if to_regclass('public.receipts') is not null and to_regclass('public.receipt_items') is not null then
                        insert into expense.receipts (id, store, purchase_date)
                        select id, btrim(store), purchase_date
                        from public.receipts;

                        insert into expense.receipt_items
                            (id, receipt_id, position, item, category, quantity, amount, discount, description)
                        select
                            id,
                            receipt_id,
                            row_number() over (partition by receipt_id order by id) - 1,
                            btrim(item),
                            btrim(category),
                            quantity,
                            round(unit_price * quantity, 2),
                            coalesce(discount, 0),
                            nullif(btrim(description), '')
                        from public.receipt_items;
                    end if;
                end
                $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "receipt_items",
                schema: "expense");

            migrationBuilder.DropTable(
                name: "receipts",
                schema: "expense");
        }
    }
}
