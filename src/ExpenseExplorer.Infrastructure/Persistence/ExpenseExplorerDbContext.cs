using ExpenseExplorer.Infrastructure.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Persistence;

internal sealed class ExpenseExplorerDbContext(DbContextOptions<ExpenseExplorerDbContext> options)
    : IdentityDbContext<UserAccount, IdentityRole<Guid>, Guid>(options)
{
    /// <summary>
    /// v2 keeps its tables in their own schema; the previous app's tables in <c>public</c>
    /// stay untouched and are only read once, by the initial migration.
    /// </summary>
    public const string Schema = "expense";

    public DbSet<ReceiptRow> Receipts => Set<ReceiptRow>();

    public DbSet<ReceiptItemRow> ReceiptItems => Set<ReceiptItemRow>();

    public DbSet<RefreshTokenRow> RefreshTokens => Set<RefreshTokenRow>();

    public DbSet<NameAliasRow> NameAliases => Set<NameAliasRow>();

    public DbSet<BudgetGroupRow> BudgetGroups => Set<BudgetGroupRow>();

    public DbSet<BudgetCategoryRow> BudgetCategories => Set<BudgetCategoryRow>();

    public DbSet<BudgetPeriodRow> BudgetPeriods => Set<BudgetPeriodRow>();

    public DbSet<BudgetFundRow> BudgetFunds => Set<BudgetFundRow>();

    public DbSet<BudgetItemRow> BudgetItems => Set<BudgetItemRow>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Schema);
        ConfigureUsers(builder);

        // Check constraints repeat the domain rules, so rows written outside the app cannot break them.
        builder.Entity<ReceiptRow>(receipt =>
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

        builder.Entity<ReceiptItemRow>(item =>
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

        builder.Entity<NameAliasRow>(alias =>
        {
            alias.ToTable("name_aliases", table =>
            {
                table.HasCheckConstraint("ck_name_aliases_kind", "kind in ('store', 'item', 'category')");
                table.HasCheckConstraint("ck_name_aliases_name_not_blank", "btrim(name) <> ''");
            });
            alias.HasKey(a => new { a.Kind, a.Alias });
            alias.Property(a => a.Kind).HasColumnName("kind").HasMaxLength(10);
            alias.Property(a => a.Alias).HasColumnName("alias").HasMaxLength(100);
            alias.Property(a => a.Name).HasColumnName("name").HasMaxLength(100);
        });

        ConfigureBudget(builder);
    }

    private static void ConfigureBudget(ModelBuilder builder)
    {
        builder.Entity<BudgetGroupRow>(group =>
        {
            group.ToTable("budget_groups", table =>
                table.HasCheckConstraint("ck_budget_groups_name_not_blank", "btrim(name) <> ''"));
            group.HasKey(g => g.Id);
            group.Property(g => g.Id).HasColumnName("id").ValueGeneratedNever();
            group.Property(g => g.Name).HasColumnName("name").HasMaxLength(100);
            group.Property(g => g.Position).HasColumnName("position");
            group.HasIndex(g => g.Name).IsUnique();
        });

        builder.Entity<BudgetCategoryRow>(category =>
        {
            category.ToTable("budget_categories");
            category.HasKey(c => c.Category);
            category.Property(c => c.Category).HasColumnName("category").HasMaxLength(100);
            category.Property(c => c.GroupId).HasColumnName("group_id");
            category.HasIndex(c => c.GroupId);
            category.HasOne<BudgetGroupRow>().WithMany().HasForeignKey(c => c.GroupId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<BudgetPeriodRow>(period =>
        {
            period.ToTable("budget_periods", table =>
                table.HasCheckConstraint("ck_budget_periods_end_after_start", "end_date >= start_date"));
            period.HasKey(p => p.Id);
            period.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();
            period.Property(p => p.Start).HasColumnName("start_date");
            period.Property(p => p.End).HasColumnName("end_date");
            period.HasIndex(p => p.Start).IsUnique();
        });

        builder.Entity<BudgetFundRow>(fund =>
        {
            fund.ToTable("budget_funds", table =>
            {
                table.HasCheckConstraint("ck_budget_funds_name_not_blank", "btrim(name) <> ''");
                table.HasCheckConstraint("ck_budget_funds_day", "day between 1 and 31");
            });
            fund.HasKey(f => f.Id);
            fund.Property(f => f.Id).HasColumnName("id").ValueGeneratedNever();
            fund.Property(f => f.PeriodId).HasColumnName("period_id");
            fund.Property(f => f.Position).HasColumnName("position");
            fund.Property(f => f.Name).HasColumnName("name").HasMaxLength(100);
            fund.Property(f => f.Amount).HasColumnName("amount").HasPrecision(12, 2);
            fund.HasIndex(f => f.PeriodId);
            fund.HasOne<BudgetPeriodRow>().WithMany().HasForeignKey(f => f.PeriodId).OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<BudgetItemRow>(item =>
        {
            item.ToTable("budget_items", table =>
            {
                table.HasCheckConstraint("ck_budget_items_name_not_blank", "btrim(name) <> ''");
                table.HasCheckConstraint("ck_budget_items_amount_not_negative", "amount >= 0 and (estimate is null or estimate >= 0)");
                table.HasCheckConstraint("ck_budget_items_day", "day between 1 and 31");
            });
            item.HasKey(i => i.Id);
            item.Property(i => i.Id).HasColumnName("id").ValueGeneratedNever();
            item.Property(i => i.PeriodId).HasColumnName("period_id");
            item.Property(i => i.GroupId).HasColumnName("group_id");
            item.Property(i => i.Position).HasColumnName("position");
            item.Property(i => i.Name).HasColumnName("name").HasMaxLength(100);
            item.Property(i => i.Amount).HasColumnName("amount").HasPrecision(12, 2);
            item.HasIndex(i => i.PeriodId);
            item.HasIndex(i => i.GroupId);
            item.HasOne<BudgetPeriodRow>().WithMany().HasForeignKey(i => i.PeriodId).OnDelete(DeleteBehavior.Cascade);
            item.HasOne<BudgetGroupRow>().WithMany().HasForeignKey(i => i.GroupId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>().ToTable("users");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("roles").HasData(IdentityRoles.All);
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");

        modelBuilder.Entity<RefreshTokenRow>(token =>
        {
            token.ToTable("refresh_tokens");
            token.HasKey(t => t.Id);
            token.Property(t => t.Id).HasColumnName("id").ValueGeneratedNever();
            token.Property(t => t.UserId).HasColumnName("user_id");
            token.Property(t => t.TokenHash).HasColumnName("token_hash").HasMaxLength(64);
            token.Property(t => t.CreatedAt).HasColumnName("created_at");
            token.Property(t => t.ExpiresAt).HasColumnName("expires_at");
            token.Property(t => t.RevokedAt).HasColumnName("revoked_at");
            token.HasIndex(t => t.TokenHash).IsUnique();
            token.HasIndex(t => t.UserId);
            token.HasOne<UserAccount>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
