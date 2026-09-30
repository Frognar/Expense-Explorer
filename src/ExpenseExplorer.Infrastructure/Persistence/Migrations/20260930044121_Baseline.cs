using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseExplorer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The previous app version created these tables itself (without EF migrations).
            // Create them only when missing, then bring an existing schema to the current model.
            migrationBuilder.Sql("""
                create table if not exists receipts (
                    id uuid primary key,
                    store text not null,
                    purchase_date date not null);

                create table if not exists receipt_items (
                    id uuid primary key,
                    receipt_id uuid not null references receipts(id),
                    item text not null,
                    category text not null,
                    unit_price numeric(15, 4) not null,
                    quantity numeric(12, 4) not null,
                    discount numeric(15, 2),
                    description text);

                update receipt_items set discount = 0 where discount is null;
                alter table receipt_items
                    alter column discount set default 0,
                    alter column discount set not null,
                    add column if not exists position integer not null default 0;

                create index if not exists ix_receipt_items_receipt_id on receipt_items (receipt_id);
                create index if not exists ix_receipts_purchase_date on receipts (purchase_date);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty: the tables may predate this migration and hold real data.
        }
    }
}
