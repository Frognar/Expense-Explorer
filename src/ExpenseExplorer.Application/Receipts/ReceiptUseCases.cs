using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Application.Receipts;

/// <summary>
/// Each use case loads what it needs, lets the domain decide, and saves only on success.
/// </summary>
public static class ReceiptUseCases
{
    public static async Task<Result<Receipt>> CreateAsync(
        IReceiptRepository receipts,
        CreateReceipt command,
        CancellationToken cancellationToken)
    {
        Receipt receipt = Receipt.Create(ReceiptId.New(), command.Store, command.PurchaseDate);
        receipts.Add(receipt);
        await receipts.SaveChangesAsync(cancellationToken);
        return Result.Success(receipt);
    }

    public static Task<Result<Receipt>> ChangeAsync(
        IReceiptRepository receipts,
        ChangeReceipt command,
        CancellationToken cancellationToken) =>
        WithReceiptAsync(
            receipts,
            command.ReceiptId,
            receipt =>
            {
                if (command.Store is not null)
                {
                    receipt.ChangeStore(command.Store);
                }

                if (command.PurchaseDate is not null)
                {
                    receipt.ChangePurchaseDate(command.PurchaseDate);
                }

                return Result.Success(receipt);
            },
            cancellationToken);

    public static Task<Result<Receipt>> DeleteAsync(
        IReceiptRepository receipts,
        DeleteReceipt command,
        CancellationToken cancellationToken) =>
        WithReceiptAsync(
            receipts,
            command.ReceiptId,
            receipt =>
            {
                receipts.Remove(receipt);
                return Result.Success(receipt);
            },
            cancellationToken);

    public static Task<Result<Receipt>> DuplicateAsync(
        IReceiptRepository receipts,
        DuplicateReceipt command,
        CancellationToken cancellationToken) =>
        WithReceiptAsync(
            receipts,
            command.ReceiptId,
            receipt =>
            {
                Receipt copy = receipt.Duplicate(ReceiptId.New(), command.PurchaseDate, ReceiptItemId.New);
                receipts.Add(copy);
                return Result.Success(copy);
            },
            cancellationToken);

    public static Task<Result<ReceiptItem>> AddItemAsync(
        IReceiptRepository receipts,
        AddReceiptItem command,
        CancellationToken cancellationToken) =>
        WithReceiptAsync(
            receipts,
            command.ReceiptId,
            receipt => receipt.AddItem(ReceiptItemId.New(), command.Purchase),
            cancellationToken);

    public static Task<Result<ReceiptItem>> ChangeItemAsync(
        IReceiptRepository receipts,
        ChangeReceiptItem command,
        CancellationToken cancellationToken) =>
        WithReceiptAsync(
            receipts,
            command.ReceiptId,
            receipt => receipt.ChangeItem(command.ItemId, command.Purchase),
            cancellationToken);

    public static Task<Result<ReceiptItem>> RemoveItemAsync(
        IReceiptRepository receipts,
        RemoveReceiptItem command,
        CancellationToken cancellationToken) =>
        WithReceiptAsync(
            receipts,
            command.ReceiptId,
            receipt => receipt.RemoveItem(command.ItemId),
            cancellationToken);

    private static async Task<Result<T>> WithReceiptAsync<T>(
        IReceiptRepository receipts,
        ReceiptId id,
        Func<Receipt, Result<T>> change,
        CancellationToken cancellationToken)
    {
        Receipt? receipt = await receipts.FindAsync(id, cancellationToken);
        if (receipt is null)
        {
            return Result.Failure<T>(ApplicationErrors.ReceiptNotFound);
        }

        Result<T> result = change(receipt);
        if (result.IsSuccess)
        {
            await receipts.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}
