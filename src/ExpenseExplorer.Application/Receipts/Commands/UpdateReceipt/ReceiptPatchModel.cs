namespace ExpenseExplorer.Application.Receipts.Commands;

using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Monads;

internal readonly record struct ReceiptPatchModel
{
  private ReceiptPatchModel(Maybe<Store> store, Maybe<NonFutureDate> purchaseDate, DateOnly today)
  {
    Store = store;
    PurchaseDate = purchaseDate;
    Today = today;
  }

  public Maybe<Store> Store { get; }

  public Maybe<NonFutureDate> PurchaseDate { get; }

  public DateOnly Today { get; }

  public static ReceiptPatchModel Create(Maybe<Store> store, Maybe<NonFutureDate> purchaseDate, DateOnly today) => new(store, purchaseDate, today);
}
