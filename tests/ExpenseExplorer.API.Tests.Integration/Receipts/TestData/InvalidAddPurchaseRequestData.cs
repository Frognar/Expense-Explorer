namespace ExpenseExplorer.API.Tests.Integration.Receipts.TestData;

using System.Collections;

public class InvalidAddPurchaseRequestData : IEnumerable<object[]>
{
  private static readonly List<object[]> _data =
  [
    [
      new
      {
        item = string.Empty,
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
        category = string.Empty,
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
        quantity = 0,
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
        unitPrice = -1,
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
        unitPrice = 0,
        totalDiscount = -1,
        description = (string?)null,
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
