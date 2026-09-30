using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Domain.Tests;

public class ReceiptTests
{
    [Fact]
    public void New_receipt_has_no_items_and_zero_total()
    {
        var receipt = Given.Receipt();

        Assert.Empty(receipt.Items);
        Assert.Equal(Money.Zero, receipt.Total);
    }

    [Fact]
    public void Total_is_the_sum_of_item_totals()
    {
        var receipt = Given.Receipt();
        receipt.AddItem(ReceiptItemId.New(), Given.Purchase(quantity: 2m, unitPrice: 3.00m, discount: 0.50m));
        receipt.AddItem(ReceiptItemId.New(), Given.Purchase(unitPrice: 4.99m));

        Assert.Equal(10.49m, receipt.Total.Value);
    }

    [Fact]
    public void Adding_an_item_with_an_existing_id_fails()
    {
        var receipt = Given.Receipt();
        var id = ReceiptItemId.New();
        receipt.AddItem(id, Given.Purchase());

        var result = receipt.AddItem(id, Given.Purchase(item: "Bread"));

        Assert.Equal(ReceiptErrors.ItemAlreadyExists, Assert.Single(result.Errors));
        Assert.Single(receipt.Items);
    }

    [Fact]
    public void Changing_an_item_replaces_its_purchase()
    {
        var receipt = Given.Receipt();
        var id = ReceiptItemId.New();
        receipt.AddItem(id, Given.Purchase());
        var bread = Given.Purchase(item: "Bread", unitPrice: 5.00m);

        var result = receipt.ChangeItem(id, bread);

        Assert.True(result.IsSuccess);
        Assert.Same(bread, Assert.Single(receipt.Items).Purchase);
    }

    [Fact]
    public void Changing_a_missing_item_fails()
    {
        var receipt = Given.Receipt();

        var result = receipt.ChangeItem(ReceiptItemId.New(), Given.Purchase());

        Assert.Equal(ReceiptErrors.ItemNotFound, Assert.Single(result.Errors));
    }

    [Fact]
    public void Removing_an_item_drops_it_from_the_receipt()
    {
        var receipt = Given.Receipt();
        var id = ReceiptItemId.New();
        receipt.AddItem(id, Given.Purchase());

        var result = receipt.RemoveItem(id);

        Assert.True(result.IsSuccess);
        Assert.Empty(receipt.Items);
    }

    [Fact]
    public void Removing_a_missing_item_fails()
    {
        var receipt = Given.Receipt();

        var result = receipt.RemoveItem(ReceiptItemId.New());

        Assert.Equal(ReceiptErrors.ItemNotFound, Assert.Single(result.Errors));
    }

    [Fact]
    public void Items_cannot_be_modified_from_outside()
    {
        var receipt = Given.Receipt();

        Assert.IsNotType<List<ReceiptItem>>(receipt.Items);
    }

    [Fact]
    public void Duplicate_copies_store_and_items_with_new_identifiers_and_date()
    {
        var receipt = Given.Receipt();
        receipt.AddItem(ReceiptItemId.New(), Given.Purchase());
        var newId = ReceiptId.New();
        var yesterday = Given.Date(Given.Today.AddDays(-1));

        var copy = receipt.Duplicate(newId, yesterday, ReceiptItemId.New);

        Assert.Equal(newId, copy.Id);
        Assert.Equal(receipt.Store, copy.Store);
        Assert.Equal(yesterday, copy.PurchaseDate);
        Assert.Equal(receipt.Total, copy.Total);
        Assert.NotEqual(receipt.Items[0].Id, copy.Items[0].Id);
        Assert.Equal(receipt.Items[0].Purchase, copy.Items[0].Purchase);
    }

    [Fact]
    public void Changes_to_a_duplicate_do_not_affect_the_original()
    {
        var receipt = Given.Receipt();
        receipt.AddItem(ReceiptItemId.New(), Given.Purchase());
        var copy = receipt.Duplicate(ReceiptId.New(), receipt.PurchaseDate, ReceiptItemId.New);

        copy.RemoveItem(copy.Items[0].Id);

        Assert.Single(receipt.Items);
    }

    [Fact]
    public void Restore_rebuilds_items_in_order()
    {
        var first = ReceiptItemId.New();
        var second = ReceiptItemId.New();

        var receipt = Given.Valid(Receipt.Restore(
            ReceiptId.New(),
            Given.Valid(StoreName.Create("Lidl")),
            Given.Date(Given.Today),
            [(first, Given.Purchase()), (second, Given.Purchase(item: "Bread"))]));

        Assert.Equal([first, second], receipt.Items.Select(item => item.Id));
    }

    [Fact]
    public void Restore_rejects_duplicate_item_identifiers()
    {
        var id = ReceiptItemId.New();

        var result = Receipt.Restore(
            ReceiptId.New(),
            Given.Valid(StoreName.Create("Lidl")),
            Given.Date(Given.Today),
            [(id, Given.Purchase()), (id, Given.Purchase())]);

        Assert.Equal(ReceiptErrors.ItemAlreadyExists, Assert.Single(result.Errors));
    }
}
