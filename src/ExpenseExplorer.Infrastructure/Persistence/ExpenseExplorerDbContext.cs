using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Persistence;

internal sealed class ExpenseExplorerDbContext(DbContextOptions<ExpenseExplorerDbContext> options)
    : DbContext(options)
{
    public DbSet<ReceiptRow> Receipts => Set<ReceiptRow>();

    public DbSet<ReceiptItemRow> ReceiptItems => Set<ReceiptItemRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Names match the tables created by the previous version of the app,
        // so the baseline migration can adopt an existing database.
        modelBuilder.Entity<ReceiptRow>(receipt =>
        {
            receipt.ToTable("receipts");
            receipt.HasKey(r => r.Id).HasName("receipts_pkey");
            receipt.Property(r => r.Id).HasColumnName("id").ValueGeneratedNever();
            receipt.Property(r => r.Store).HasColumnName("store");
            receipt.Property(r => r.PurchaseDate).HasColumnName("purchase_date");
            receipt.HasIndex(r => r.PurchaseDate).HasDatabaseName("ix_receipts_purchase_date");

            receipt.HasMany(r => r.Items)
                .WithOne()
                .HasForeignKey(i => i.ReceiptId)
                .HasConstraintName("receipt_items_receipt_id_fkey")
                .OnDelete(DeleteBehavior.ClientCascade);
        });

        modelBuilder.Entity<ReceiptItemRow>(item =>
        {
            item.ToTable("receipt_items");
            item.HasKey(i => i.Id).HasName("receipt_items_pkey");
            item.Property(i => i.Id).HasColumnName("id").ValueGeneratedNever();
            item.Property(i => i.ReceiptId).HasColumnName("receipt_id");
            item.Property(i => i.Position).HasColumnName("position").HasDefaultValue(0);
            item.Property(i => i.Item).HasColumnName("item");
            item.Property(i => i.Category).HasColumnName("category");
            item.Property(i => i.Quantity).HasColumnName("quantity").HasPrecision(12, 4);
            item.Property(i => i.UnitPrice).HasColumnName("unit_price").HasPrecision(15, 4);
            item.Property(i => i.Discount).HasColumnName("discount").HasPrecision(15, 2).HasDefaultValue(0m);
            item.Property(i => i.Description).HasColumnName("description");
            item.HasIndex(i => i.ReceiptId).HasDatabaseName("ix_receipt_items_receipt_id");
        });
    }
}
