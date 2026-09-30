using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Application.Receipts;

/// <summary>Loads and stores whole <see cref="Receipt"/> aggregates.</summary>
public interface IReceiptRepository
{
    Task<Receipt?> FindAsync(ReceiptId id, CancellationToken cancellationToken);

    void Add(Receipt receipt);

    void Remove(Receipt receipt);

    /// <summary>Persists every receipt that was found, added or removed through this repository.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
