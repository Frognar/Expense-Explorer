using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.Receipts;

namespace ExpenseExplorer.Application.Features.Receipts.UpdateHeader;

public class UpdateReceiptHeaderCommandHandler(
    IUpdateReceiptHeaderPersistence persistence)
    : ICommandHandler<UpdateReceiptHeaderCommand, UpdateReceiptHeaderResponse>
{
    public async Task<Result<UpdateReceiptHeaderResponse>> HandleAsync(UpdateReceiptHeaderCommand command,
        CancellationToken cancellationToken)
    {
        return await persistence.GetReceiptByIdAsync(command.ReceiptId, cancellationToken)
            .BindAsync(r => UpdateReceipt(r, command))
            .BindAsync(async r => await persistence.SaveReceiptAsync(r, cancellationToken))
            .MapAsync(_ => new UpdateReceiptHeaderResponse());
    }

    private static Result<Receipt> UpdateReceipt(Receipt receipt, UpdateReceiptHeaderCommand command)
    {
        return receipt.ToResult()
            .Bind(r => r.UpdateStore(command.Store))
            .Bind(r => r.UpdatePurchaseDate(command.PurchaseDate));
    }
}