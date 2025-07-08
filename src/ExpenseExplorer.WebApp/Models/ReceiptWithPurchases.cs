using System.Globalization;
using System.Text;

namespace ExpenseExplorer.WebApp.Models;

internal sealed record ReceiptWithPurchases(
    Guid Id,
    string Store,
    DateOnly PurchaseDate,
    IEnumerable<PurchaseDetails> Purchases)
{
    public byte[] ToCsv()
    {
        StringBuilder sb = new();
        sb.AppendLine("Store,PurchaseDate");
        CultureInfo cultureInfo = new("pl-PL");
        sb.AppendLine(cultureInfo, $"{Store},{PurchaseDate:yyyy-MM-dd}");
        sb.AppendLine();
        sb.AppendLine("ItemName,Category,Quantity,UnitPrice,PriceBeforeDiscount,Discount,PriceAfterDiscount,Description");
        IEnumerable<string> strings = Purchases.Select(p =>
            string.Join(",",
                p.ItemName,
                p.Category,
                p.Quantity.ToString(cultureInfo).Replace(",", ".", StringComparison.CurrentCulture),
                p.UnitPrice.ToString("c", cultureInfo).Replace(",", ".", StringComparison.CurrentCulture),
                p.PriceBeforeDiscount.ToString("c", cultureInfo).Replace(",", ".", StringComparison.CurrentCulture),
                p.Discount?.ToString("c", cultureInfo).Replace(",", ".", StringComparison.CurrentCulture) ?? "",
                p.PriceAfterDiscount.ToString("c", cultureInfo).Replace(",", ".", StringComparison.CurrentCulture),
                p.Description ?? ""
            ));

        foreach (string s in strings)
        {
            sb.AppendLine(s);
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}