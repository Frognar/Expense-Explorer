using DotMaybe;
using DotResult;
using DotValid;
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

    private static Validated<ReceiptItemId> ValidateReceiptItemId(Guid receiptItemId) =>
        ReceiptItemId.TryCreate(receiptItemId)
            .ToValidated(() => new ValidationError(nameof(receiptItemId), ErrorCodes.InvalidReceiptItemId));

    private static Validated<ReceiptId> ValidateReceiptId(Guid receiptId) =>
        ReceiptId.TryCreate(receiptId)
            .ToValidated(() => new ValidationError(nameof(receiptId), ErrorCodes.InvalidReceiptId));

    private static Validated<Item> ValidateItem(string itemName) =>
        Item.TryCreate(itemName)
            .ToValidated(() => new ValidationError(nameof(itemName), ErrorCodes.EmptyItemName));

    private static Validated<Category> ValidateCategory(string categoryName) =>
        Category.TryCreate(categoryName)
            .ToValidated(() => new ValidationError(nameof(categoryName), ErrorCodes.EmptyCategoryName));

    private static Validated<Quantity> ValidateQuantity(decimal quantity) =>
        Quantity.TryCreate(quantity)
            .ToValidated(() => new ValidationError(nameof(quantity), ErrorCodes.InvalidQuantity));

    private static Validated<Money> ValidateUnitPrice(decimal unitPrice) =>
        Money.TryCreate(unitPrice)
            .ToValidated(() => new ValidationError(nameof(unitPrice), ErrorCodes.InvalidUnitPrice));

    private static Validated<Maybe<Money>> ValidateDiscount(decimal? discount) =>
        discount is null
            ? Validation.Succeed(None.OfType<Money>())
            : Money.TryCreate(discount.Value)
                .Match(
                    none: () => Validation.Failed<Maybe<Money>>([
                        new ValidationError(nameof(discount), ErrorCodes.InvalidDiscount)
                    ]),
                    some: money => Validation.Succeed(Some.With(money)));

    private static Validated<Maybe<Description>> ValidateDescription(string? description) =>
        description is null
            ? Validation.Succeed(None.OfType<Description>())
            : Validation.Succeed(Some.With(new Description(description)));
}