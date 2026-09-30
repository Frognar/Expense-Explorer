using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Receipts;

/// <summary>
/// Keeps each loaded receipt next to its tracked row and copies the domain state
/// onto the row just before saving.
/// </summary>
internal sealed class ReceiptRepository(ExpenseExplorerDbContext db) : IReceiptRepository
{
    private readonly Dictionary<ReceiptId, (Receipt Receipt, ReceiptRow Row)> _tracked = [];

    public async Task<Receipt?> FindAsync(ReceiptId id, CancellationToken cancellationToken)
    {
        ReceiptRow? row = await db.Receipts
            .Include(r => r.Items)
            .SingleOrDefaultAsync(r => r.Id == id.Value, cancellationToken);

        if (row is null)
        {
            return null;
        }

        Receipt receipt = ReceiptMapping.ToDomain(row);
        _tracked[receipt.Id] = (receipt, row);
        return receipt;
    }

    public void Add(Receipt receipt)
    {
        ReceiptRow row = ReceiptMapping.ToNewRow(receipt);
        db.Receipts.Add(row);
        _tracked[receipt.Id] = (receipt, row);
    }

    public void Remove(Receipt receipt)
    {
        if (_tracked.Remove(receipt.Id, out (Receipt Receipt, ReceiptRow Row) entry))
        {
            db.Receipts.Remove(entry.Row);
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        foreach ((Receipt receipt, ReceiptRow row) in _tracked.Values)
        {
            ReceiptMapping.CopyTo(receipt, row);
        }

        return db.SaveChangesAsync(cancellationToken);
    }
}
