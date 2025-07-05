using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.Receipts;

namespace ExpenseExplorer.Application.Features.Receipts.UpdateItem;

internal sealed class UpdateReceiptItemCommandHandler(
    IUpdateReceiptItemPersistence persistence)
    : ICommandHandler<UpdateReceiptItemCommand, UpdateReceiptItemResponse>
{
    public async Task<Result<UpdateReceiptItemResponse>> HandleAsync(UpdateReceiptItemCommand command,
        CancellationToken cancellationToken)
    {
        return await persistence.GetReceiptByIdAsync(command.ReceiptId, cancellationToken)
            .BindAsync(r => UpdateItemToReceipt(r, command))
            .BindAsync(async r => await persistence.SaveReceiptAsync(r, cancellationToken))
            .MapAsync(_ => new UpdateReceiptItemResponse());
    }

    private static Result<Receipt> UpdateItemToReceipt(Receipt receipt, UpdateReceiptItemCommand command) =>
        receipt.UpdateItem(new ReceiptItem(
            command.ReceiptItemId,
            command.ReceiptId,
            command.Item,
            command.Category,
            command.Quantity,
            command.UnitPrice,
            command.Discount,
            command.Description));
}