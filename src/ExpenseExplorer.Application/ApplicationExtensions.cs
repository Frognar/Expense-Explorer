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
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseExplorer.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddScoped<ICommandHandler<CreateReceiptHeaderRequest, CreateReceiptHeaderResponse>>(
                sp => new CreateReceiptHeaderCommandValidator(
                    new CreateReceiptHeaderCommandHandler(
                        sp.GetRequiredService<ICreateReceiptHeaderPersistence>())))
            .AddScoped<ICommandHandler<UpdateReceiptHeaderRequest, UpdateReceiptHeaderResponse>>(
                sp => new UpdateReceiptHeaderCommandValidator(
                    new UpdateReceiptHeaderCommandHandler(
                        sp.GetRequiredService<IUpdateReceiptHeaderPersistence>())))
            .AddScoped<ICommandHandler<DeleteReceiptHeaderRequest, DeleteReceiptHeaderResponse>>(
                sp => new DeleteReceiptHeaderCommandValidator(
                    new DeleteReceiptHeaderCommandHandler(
                        sp.GetRequiredService<IDeleteReceiptHeaderPersistence>())))
            .AddScoped<ICommandHandler<DuplicateReceiptRequest, DuplicateReceiptResponse>>(
                sp => new DuplicateReceiptCommandValidator(
                    new DuplicateReceiptCommandHandler(
                        sp.GetRequiredService<IDuplicateReceiptPersistence>())))
            .AddScoped<ICommandHandler<AddReceiptItemRequest, AddReceiptItemResponse>>(
                sp => new AddReceiptItemCommandValidator(
                    new AddReceiptItemCommandHandler(
                        sp.GetRequiredService<IAddReceiptItemPersistence>())))
            .AddScoped<ICommandHandler<UpdateReceiptItemRequest, UpdateReceiptItemResponse>>(
                sp => new UpdateReceiptItemCommandValidator(
                    new UpdateReceiptItemCommandHandler(
                        sp.GetRequiredService<IUpdateReceiptItemPersistence>())))
            .AddScoped<ICommandHandler<DeleteReceiptItemRequest, DeleteReceiptItemResponse>>(
                sp => new DeleteReceiptItemCommandValidator(
                    new DeleteReceiptItemCommandHandler(
                        sp.GetRequiredService<IDeleteReceiptItemPersistence>())))
            .AddScoped<IQueryHandler<GetReceiptByIdQuery, GetReceiptByIdResponse>, GetReceiptByIdHandler>()
            .AddScoped<IQueryHandler<GetReceiptSummariesQuery, GetReceiptSummariesResponse>, GetReceiptSummariesQueryHandler>();
    }
}