using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.Receipts;

namespace ExpenseExplorer.Application.Features.Receipts.AddItem;

internal sealed class AddReceiptItemCommandHandler(
    IAddReceiptItemPersistence persistence)
    : ICommandHandler<AddReceiptItemCommand, AddReceiptItemResponse>
{
    public async Task<Result<AddReceiptItemResponse>> HandleAsync(AddReceiptItemCommand command, CancellationToken cancellationToken)
    {
        return await persistence.GetReceiptByIdAsync(command.ReceiptId, cancellationToken)
                .BindAsync(r => AddItemToReceipt(r, command))
                .BindAsync(async r => await persistence.SaveReceiptAsync(r, cancellationToken))
                .MapAsync(_ => new AddReceiptItemResponse(command.Id.Value));
    }

    private static Result<Receipt> AddItemToReceipt(Receipt receipt, AddReceiptItemCommand command)
    {
        return receipt.AddItem(
            command.Id,
            command.Item,
            command.Category,
            command.Quantity,
            command.UnitPrice,
            command.Discount,
            command.Description);
    }
}