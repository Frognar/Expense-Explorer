namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

internal sealed record ReceiptPatchModel(Maybe<Store> Store, Maybe<PurchaseDate> PurchaseDate, DateOnly Today)
{
  public static ReceiptPatchModel Create(Maybe<Store> store, Maybe<PurchaseDate> purchaseDate, DateOnly today)
    => new(store, purchaseDate, today);
}