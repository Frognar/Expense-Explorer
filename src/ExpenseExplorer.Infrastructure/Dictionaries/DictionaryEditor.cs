using ExpenseExplorer.Application.Dictionaries;
using ExpenseExplorer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpenseExplorer.Infrastructure.Dictionaries;

internal sealed class DictionaryEditor(ExpenseExplorerDbContext db) : IDictionaryEditor
{
    public async Task<RenamedName?> RenameAsync(RenameName command, CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        bool merged = command.From != command.To && await IsUsedAsync(command.Kind, command.To, cancellationToken);
        int changed = await ReplaceAsync(command, cancellationToken);
        if (changed == 0)
        {
            return null;
        }

        await RememberAliasAsync(command, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new RenamedName(command.To, changed, merged);
    }

    public async Task<IReadOnlyList<NameUsage>> UsageAsync(NameKind kind, DictionaryQuery query, CancellationToken cancellationToken)
    {
        IQueryable<NameUse> uses = Uses(kind);
        if (query.Search.Length > 0)
        {
            string pattern = $"%{DictionaryQueries.EscapeLike(query.Search)}%";
            uses = uses.Where(use => EF.Functions.ILike(use.Name, pattern, @"\"));
        }

        return await uses
            .GroupBy(use => use.Name)
            .OrderBy(name => name.Key)
            .Take(query.Limit)
            .Select(name => new NameUsage(name.Key, name.Count(), name.Max(use => use.PurchaseDate)))
            .ToListAsync(cancellationToken);
    }

    /// <summary>Every place the name appears, with the date of the receipt it is on.</summary>
    private IQueryable<NameUse> Uses(NameKind kind) => kind switch
    {
        NameKind.Store => db.Receipts.AsNoTracking()
            .Select(receipt => new NameUse { Name = receipt.Store, PurchaseDate = receipt.PurchaseDate }),
        NameKind.Item =>
            from item in db.ReceiptItems.AsNoTracking()
            join receipt in db.Receipts.AsNoTracking() on item.ReceiptId equals receipt.Id
            select new NameUse { Name = item.Item, PurchaseDate = receipt.PurchaseDate },
        _ =>
            from item in db.ReceiptItems.AsNoTracking()
            join receipt in db.Receipts.AsNoTracking() on item.ReceiptId equals receipt.Id
            select new NameUse { Name = item.Category, PurchaseDate = receipt.PurchaseDate },
    };

    private Task<bool> IsUsedAsync(NameKind kind, string name, CancellationToken cancellationToken) => kind switch
    {
        NameKind.Store => db.Receipts.AnyAsync(receipt => receipt.Store == name, cancellationToken),
        NameKind.Item => db.ReceiptItems.AnyAsync(item => item.Item == name, cancellationToken),
        _ => db.ReceiptItems.AnyAsync(item => item.Category == name, cancellationToken),
    };

    private Task<int> ReplaceAsync(RenameName command, CancellationToken cancellationToken) => command.Kind switch
    {
        NameKind.Store => db.Receipts
            .Where(receipt => receipt.Store == command.From)
            .ExecuteUpdateAsync(set => set.SetProperty(receipt => receipt.Store, command.To), cancellationToken),
        NameKind.Item => db.ReceiptItems
            .Where(item => item.Item == command.From)
            .ExecuteUpdateAsync(set => set.SetProperty(item => item.Item, command.To), cancellationToken),
        _ => db.ReceiptItems
            .Where(item => item.Category == command.From)
            .ExecuteUpdateAsync(set => set.SetProperty(item => item.Category, command.To), cancellationToken),
    };

    /// <summary>
    /// The old name now leads to the new one, and so does every older name that led to the old one.
    /// The new name stops being an alias of anything else, since it is a name in use now.
    /// </summary>
    private async Task RememberAliasAsync(RenameName command, CancellationToken cancellationToken)
    {
        string kind = NameColumns.Code(command.Kind);
        string from = NameAliases.Key(command.From);
        string to = NameAliases.Key(command.To);

        if (from != to)
        {
            await db.NameAliases
                .Where(alias => alias.Kind == kind && alias.Alias == to)
                .ExecuteDeleteAsync(cancellationToken);
        }

        await db.NameAliases
            .Where(alias => alias.Kind == kind && alias.Name == command.From)
            .ExecuteUpdateAsync(set => set.SetProperty(alias => alias.Name, command.To), cancellationToken);

        await db.Database.ExecuteSqlAsync(
            $"""
            insert into expense.name_aliases (kind, alias, name) values ({kind}, {from}, {command.To})
            on conflict (kind, alias) do update set name = excluded.name
            """,
            cancellationToken);
    }

    private sealed class NameUse
    {
        public string Name { get; init; } = "";

        public DateOnly PurchaseDate { get; init; }
    }
}
