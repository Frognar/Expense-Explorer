using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Features.Receipts.AddItem;
using ExpenseExplorer.Application.Features.Receipts.CreateHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteHeader;
using ExpenseExplorer.Application.Features.Receipts.DeleteItem;
using ExpenseExplorer.Application.Features.Receipts.Duplicate;
using ExpenseExplorer.Application.Features.Receipts.GetReceipt;
using ExpenseExplorer.Application.Features.Receipts.GetReceipts;
using ExpenseExplorer.Application.Features.Receipts.UpdateHeader;
using ExpenseExplorer.Application.Features.Receipts.UpdateItem;

namespace ExpenseExplorer.Application.Features.Receipts;

public static class ReceiptsFeaturesFactory
{
    public static ICommandHandler<CreateReceiptHeaderRequest, CreateReceiptHeaderResponse>
        CreateCreateReceiptHeaderHandler(ICreateReceiptHeaderPersistence createReceiptHeaderPersistence)
        => new CreateReceiptHeaderCommandValidator(
            new CreateReceiptHeaderCommandHandler(createReceiptHeaderPersistence));

    public static ICommandHandler<UpdateReceiptHeaderRequest, UpdateReceiptHeaderResponse>
        CreateUpdateReceiptHeaderHandler(IUpdateReceiptHeaderPersistence updateReceiptHeaderPersistence)
        => new UpdateReceiptHeaderCommandValidator(
            new UpdateReceiptHeaderCommandHandler(updateReceiptHeaderPersistence));

    public static ICommandHandler<DeleteReceiptHeaderRequest, DeleteReceiptHeaderResponse>
        CreateDeleteReceiptHeaderHandler(IDeleteReceiptHeaderPersistence deleteReceiptHeaderPersistence)
        => new DeleteReceiptHeaderCommandValidator(
            new DeleteReceiptHeaderCommandHandler(deleteReceiptHeaderPersistence));

    public static ICommandHandler<DuplicateReceiptRequest, DuplicateReceiptResponse>
        CreateDuplicateReceiptHandler(IDuplicateReceiptPersistence duplicateReceiptPersistence)
        => new DuplicateReceiptCommandValidator(
            new DuplicateReceiptCommandHandler(duplicateReceiptPersistence));

    public static ICommandHandler<AddReceiptItemRequest, AddReceiptItemResponse>
        CreateAddReceiptItemHandler(IAddReceiptItemPersistence addReceiptItemPersistence)
        => new AddReceiptItemCommandValidator(
            new AddReceiptItemCommandHandler(addReceiptItemPersistence));

    public static ICommandHandler<UpdateReceiptItemRequest, UpdateReceiptItemResponse>
        CreateUpdateReceiptItemHandler(IUpdateReceiptItemPersistence updateReceiptItemPersistence)
        => new UpdateReceiptItemCommandValidator(
            new UpdateReceiptItemCommandHandler(updateReceiptItemPersistence));

    public static ICommandHandler<DeleteReceiptItemRequest, DeleteReceiptItemResponse>
        CreateDeleteReceiptItemHandler(IDeleteReceiptItemPersistence deleteReceiptItemPersistence)
        => new DeleteReceiptItemCommandValidator(
            new DeleteReceiptItemCommandHandler(deleteReceiptItemPersistence));

    public static IQueryHandler<GetReceiptByIdQuery, GetReceiptByIdResponse>
        CreateGetReceiptByIdHandler(IGetReceiptByIdPersistence getReceiptByIdPersistence)
        => new GetReceiptByIdHandler(getReceiptByIdPersistence);

    public static IQueryHandler<GetReceiptSummariesQuery, GetReceiptSummariesResponse>
        CreateGetReceiptSummariesHandler(IGetReceiptSummariesPersistence getReceiptSummariesPersistence)
        => new GetReceiptSummariesQueryHandler(getReceiptSummariesPersistence);
}