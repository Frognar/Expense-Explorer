using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Contracts.Common;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Api.Receipts;

/// <summary>
/// Pure functions from raw requests to commands. Every problem in the input is reported
/// at once, tagged with the name of the field it concerns.
/// </summary>
internal static class ReceiptRequestParser
{
    public static Result<CreateReceipt> ParseCreate(CreateReceiptRequest request, DateOnly today) =>
        ResultCombine.Combine(
            StoreName.Create(request.Store).ForTarget("store"),
            RequiredDate(request.PurchaseDate, today).ForTarget("purchaseDate"),
            (store, date) => new CreateReceipt(store, date));

    public static Result<ChangeReceipt> ParseChange(Guid receiptId, UpdateReceiptRequest request, DateOnly today) =>
        ResultCombine.Combine(
            ReceiptId(receiptId),
            Input.Optional(request.Store, StoreName.Create).ForTarget("store"),
            Input.Optional(request.PurchaseDate, date => PurchaseDate.Create(date, today)).ForTarget("purchaseDate"),
            (id, store, date) => new ChangeReceipt(id, store, date));

    public static Result<DeleteReceipt> ParseDelete(Guid receiptId) =>
        ReceiptId(receiptId).Map(id => new DeleteReceipt(id));

    public static Result<DuplicateReceipt> ParseDuplicate(Guid receiptId, DuplicateReceiptRequest request, DateOnly today) =>
        ResultCombine.Combine(
            ReceiptId(receiptId),
            RequiredDate(request.PurchaseDate, today).ForTarget("purchaseDate"),
            (id, date) => new DuplicateReceipt(id, date));

    public static Result<AddReceiptItem> ParseAddItem(Guid receiptId, ReceiptItemRequest request) =>
        ResultCombine.Combine(
            ReceiptId(receiptId),
            Purchase(request),
            (id, purchase) => new AddReceiptItem(id, purchase));

    public static Result<ChangeReceiptItem> ParseChangeItem(Guid receiptId, Guid itemId, ReceiptItemRequest request) =>
        ResultCombine.Combine(
            ReceiptId(receiptId),
            ItemId(itemId),
            Purchase(request),
            (id, item, purchase) => new ChangeReceiptItem(id, item, purchase));

    public static Result<RemoveReceiptItem> ParseRemoveItem(Guid receiptId, Guid itemId) =>
        ResultCombine.Combine(
            ReceiptId(receiptId),
            ItemId(itemId),
            (id, item) => new RemoveReceiptItem(id, item));

    public static Result<ReceiptListQuery> ParseList(ReceiptListRequest request) =>
        ResultCombine.Combine(
            Input.OrderedRange(request.From, request.To).ForTarget("from"),
            Input.OrderedRange(request.TotalMin, request.TotalMax).ForTarget("totalMin"),
            Input.InRange(request.Page, 1, 1, int.MaxValue).ForTarget("page"),
            Input.InRange(request.PageSize, ReceiptListRequest.DefaultPageSize, 1, ReceiptListRequest.MaxPageSize).ForTarget("pageSize"),
            (dates, totals, page, pageSize) => new ReceiptListQuery(
                NonBlankDistinct(request.Stores),
                dates.From,
                dates.To,
                totals.From,
                totals.To,
                request.SortBy ?? ReceiptSortField.PurchaseDate,
                request.Direction ?? SortDirection.Descending,
                page,
                pageSize));

    private static Result<Purchase> Purchase(ReceiptItemRequest request) =>
        ResultCombine.Combine(
            ItemName.Create(request.Item).ForTarget("item"),
            CategoryName.Create(request.Category).ForTarget("category"),
            LinePrice(request),
            Description.CreateOptional(request.Description).ForTarget("description"),
            (item, category, price, description) => new Purchase(item, category, price, description));

    private static Result<LinePrice> LinePrice(ReceiptItemRequest request) =>
        ResultCombine.Combine(
                Input.Required(request.Quantity).Bind(Quantity.Create).ForTarget("quantity"),
                Input.Required(request.Amount).Bind(Money.Create).ForTarget("amount"),
                Money.Create(request.Discount ?? 0m).ForTarget("discount"),
                (quantity, amount, discount) => (quantity, amount, discount))
            .Bind(parts => Domain.Receipts.LinePrice.Create(parts.quantity, parts.amount, parts.discount).ForTarget("discount"));

    private static Result<PurchaseDate> RequiredDate(DateOnly? value, DateOnly today) =>
        Input.Required(value).Bind(date => PurchaseDate.Create(date, today));

    private static Result<ReceiptId> ReceiptId(Guid value) =>
        Domain.Receipts.ReceiptId.Create(value).ForTarget("receiptId");

    private static Result<ReceiptItemId> ItemId(Guid value) =>
        ReceiptItemId.Create(value).ForTarget("itemId");

    private static string[] NonBlankDistinct(IEnumerable<string>? values) =>
        [.. (values ?? []).Select(value => value.Trim()).Where(value => value.Length > 0).Distinct(StringComparer.Ordinal)];
}
