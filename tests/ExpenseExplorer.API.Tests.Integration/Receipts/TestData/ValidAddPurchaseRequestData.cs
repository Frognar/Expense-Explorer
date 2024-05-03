namespace ExpenseExplorer.API.Tests.Integration.Receipts.TestData;

public class ValidAddPurchaseRequestData : IEnumerable<object[]>
{
  private static readonly List<object[]> _data =
  [
    [
      new
      {
        item = "item",
        category = "category",
        quantity = 1,
        unitPrice = 0,
        totalDiscount = (decimal?)null,
        description = (string?)null,
      }
    ],
    [
      new
      {
        item = "item",
        category = "category",
        quantity = 1,
        unitPrice = 1,
        totalDiscount = 0,
        description = string.Empty,
      }
    ],
    [
      new
      {
        item = "item",
        category = "category",
        quantity = 1,
        unitPrice = 1,
        totalDiscount = 1,
        description = "description",
      }
    ],
  ];

  public IEnumerator<object[]> GetEnumerator()
  {
    return _data.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
