using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Persistence;

internal sealed class ExpenseExplorerDbContext(DbContextOptions<ExpenseExplorerDbContext> options)
    : DbContext(options)
{
    /// <summary>
    /// v2 keeps its tables in their own schema; the previous app's tables in <c>public</c>
    /// stay untouched and are only read once, by the initial migration.
    /// </summary>
    public const string Schema = "expense";

    public DbSet<ReceiptRow> Receipts => Set<ReceiptRow>();

    public DbSet<ReceiptItemRow> ReceiptItems => Set<ReceiptItemRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        // Check constraints repeat the domain rules, so rows written outside the app cannot break them.
        modelBuilder.Entity<ReceiptRow>(receipt =>
        {
            receipt.ToTable("receipts", table =>
                table.HasCheckConstraint("ck_receipts_store_not_blank", "btrim(store) <> ''"));
            receipt.HasKey(r => r.Id);
            receipt.Property(r => r.Id).HasColumnName("id").ValueGeneratedNever();
            receipt.Property(r => r.Store).HasColumnName("store").HasMaxLength(100);
            receipt.Property(r => r.PurchaseDate).HasColumnName("purchase_date");
            receipt.HasIndex(r => r.PurchaseDate);
            receipt.HasIndex(r => r.Store);

            receipt.HasMany(r => r.Items)
                .WithOne()
                .HasForeignKey(i => i.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReceiptItemRow>(item =>
        {
            item.ToTable("receipt_items", table =>
            {
                table.HasCheckConstraint("ck_receipt_items_item_not_blank", "btrim(item) <> ''");
                table.HasCheckConstraint("ck_receipt_items_category_not_blank", "btrim(category) <> ''");
                table.HasCheckConstraint("ck_receipt_items_quantity_positive", "quantity > 0");
                table.HasCheckConstraint("ck_receipt_items_amount_not_negative", "amount >= 0");
                table.HasCheckConstraint("ck_receipt_items_discount_within_amount", "discount >= 0 and discount <= amount");
            });
            item.HasKey(i => i.Id);
            item.Property(i => i.Id).HasColumnName("id").ValueGeneratedNever();
            item.Property(i => i.ReceiptId).HasColumnName("receipt_id");
            item.Property(i => i.Position).HasColumnName("position");
            item.Property(i => i.Item).HasColumnName("item").HasMaxLength(100);
            item.Property(i => i.Category).HasColumnName("category").HasMaxLength(100);
            item.Property(i => i.Quantity).HasColumnName("quantity").HasPrecision(12, 4);
            item.Property(i => i.Amount).HasColumnName("amount").HasPrecision(12, 2);
            item.Property(i => i.Discount).HasColumnName("discount").HasPrecision(12, 2);
            item.Property(i => i.Description).HasColumnName("description").HasMaxLength(500);
            item.HasIndex(i => i.ReceiptId);
            item.HasIndex(i => i.Item);
            item.HasIndex(i => i.Category);
        });
    }
}
