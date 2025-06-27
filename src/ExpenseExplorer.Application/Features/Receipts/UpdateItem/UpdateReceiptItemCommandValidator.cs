using DotMaybe;
using DotResult;
using ExpenseExplorer.Application.Abstractions.Messaging;
using ExpenseExplorer.Application.Domain.ValueObjects;

namespace ExpenseExplorer.Application.Features.Receipts.UpdateItem;

internal sealed class UpdateReceiptItemCommandValidator(
    ICommandHandler<UpdateReceiptItemCommand, UpdateReceiptItemResponse> inner)
    : ICommandHandler<UpdateReceiptItemRequest, UpdateReceiptItemResponse>
{
    public Task<Result<UpdateReceiptItemResponse>> HandleAsync(UpdateReceiptItemRequest command,
        CancellationToken cancellationToken)
    {
        return UpdateReceiptItemCommandFactory.Create
            .Apply(ValidateReceiptItemId(command.ReceiptItemId))
            .Apply(ValidateReceiptId(command.ReceiptId))
            .Apply(ValidateItem(command.ItemName))
            .Apply(ValidateCategory(command.CategoryName))
            .Apply(ValidateQuantity(command.Quantity))
            .Apply(ValidateUnitPrice(command.UnitPrice))
            .Apply(ValidateDiscount(command.Discount))
            .Apply(ValidateDescription(command.Description))
            .ToResult()
            .BindAsync(cmd => inner.HandleAsync(cmd, cancellationToken));
    }

    private static Validated<ReceiptItemId> ValidateReceiptItemId(Guid receiptItemId)
    {
        return ReceiptItemId.TryCreate(receiptItemId)
            .ToValidated(() => new ValidationError(nameof(receiptItemId), "Receipt item ID cannot be empty"));
    }

    private static Validated<ReceiptId> ValidateReceiptId(Guid receiptId)
    {
        return ReceiptId.TryCreate(receiptId)
            .ToValidated(() => new ValidationError(nameof(receiptId), "Receipt ID cannot be empty"));
    }

    private static Validated<Item> ValidateItem(string itemName) =>
        Item.TryCreate(itemName)
            .ToValidated(() => new ValidationError(nameof(itemName), "Item name cannot be empty"));

    private static Validated<Category> ValidateCategory(string categoryName) =>
        Category.TryCreate(categoryName)
            .ToValidated(() => new ValidationError(nameof(categoryName), "Category name cannot be empty"));

    private static Validated<Quantity> ValidateQuantity(decimal quantity) =>
        Quantity.TryCreate(quantity)
            .ToValidated(() => new ValidationError(nameof(quantity), "Quantity must be greater than zero"));

    private static Validated<Money> ValidateUnitPrice(decimal unitPrice) =>
        Money.TryCreate(unitPrice)
            .ToValidated(() => new ValidationError(nameof(unitPrice), "Unit price must be greater than zero"));

    private static Validated<Maybe<Money>> ValidateDiscount(decimal? discount) =>
        discount is null
            ? Validation.Succeed(None.OfType<Money>())
            : Money.TryCreate(discount.Value)
                .Match(
                    none: () => Validation.Failed<Maybe<Money>>([
                        new ValidationError(nameof(discount), "Discount must be greater than zero")
                    ]),
                    some: money => Validation.Succeed(Some.With(money)));

    private static Validated<Maybe<Description>> ValidateDescription(string? description) =>
        description is null
            ? Validation.Succeed(None.OfType<Description>())
            : Validation.Succeed(Some.With(new Description(description)));
}