using ExpenseExplorer.Application.Dictionaries;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Receipts;

namespace ExpenseExplorer.Api.Tests;

public class ImportHintsTests
{
    private static readonly NameAliases Aliases = new(
    [
        (NameKind.Store, "BIEDRONKA", "Biedronka Kraków"),
        (NameKind.Item, "MLEK WYPAS 3,2 1L", "Mleko 3,2%"),
        (NameKind.Category, "SPOŻYWCZE", "Jedzenie"),
    ]);

    [Fact]
    public void Renamed_names_and_known_categories_replace_what_the_import_read()
    {
        ImportReceipt receipt = Receipt("Biedronka", ("mlek wypas 3,2 1l", "Spożywcze"), ("Chleb", "Spożywcze"));

        ImportReceipt hinted = ImportHints.Apply(
            receipt,
            Aliases,
            new Dictionary<string, string> { ["Mleko 3,2%"] = "Nabiał" });

        Assert.Equal("Biedronka Kraków", hinted.Store.Value);
        Assert.Equal(
            [("Mleko 3,2%", "Nabiał"), ("Chleb", "Jedzenie")],
            hinted.Purchases.Select(purchase => (purchase.Item.Value, purchase.Category.Value)));
        Assert.Equal(receipt.Purchases.Select(purchase => purchase.Price), hinted.Purchases.Select(purchase => purchase.Price));
    }

    [Fact]
    public void Without_history_the_import_stays_as_read()
    {
        ImportReceipt receipt = Receipt("Dino", ("Masło", "Spożywcze"));

        ImportReceipt hinted = ImportHints.Apply(receipt, NameAliases.None, new Dictionary<string, string>());

        Assert.Equal(receipt.Store, hinted.Store);
        Assert.Equal(receipt.Purchases, hinted.Purchases);
    }

    private static ImportReceipt Receipt(string store, params (string Item, string Category)[] lines) =>
        new(
            Valid(StoreName.Create(store)),
            Valid(PurchaseDate.Create(new DateOnly(2026, 9, 1), ApiFixture.Today)),
            [.. lines.Select(line => new Purchase(
                Valid(ItemName.Create(line.Item)),
                Valid(CategoryName.Create(line.Category)),
                Valid(Quantity.Create(1m).Bind(quantity => Money.Create(5m).Bind(amount => LinePrice.Create(quantity, amount, Valid(Money.Create(0m)))))),
                null))]);

    private static T Valid<T>(Result<T> result) =>
        result.Match(value => value, errors => throw new InvalidOperationException(errors[0].Code));
}
