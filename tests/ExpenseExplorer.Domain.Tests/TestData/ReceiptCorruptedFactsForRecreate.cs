namespace ExpenseExplorer.Domain.Tests.TestData;

using System.Collections;
using ExpenseExplorer.Domain.Receipts.Facts;

public sealed class ReceiptCorruptedFactsForRecreate : IEnumerable<object[]>
{
  private static readonly List<object[]> _data =
  [
    [new Fact[] { new ReceiptCreated("id", string.Empty, new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)) }],
    [new Fact[] { new ReceiptCreated("id", "item", new DateOnly(2000, 1, 2), new DateOnly(2000, 1, 1)) }],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new StoreCorrected("id", string.Empty),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseDateChanged("id", new DateOnly(2000, 1, 2), new DateOnly(2000, 1, 1)),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseAdded("id", "pId", string.Empty, "c", 1, 1, 0, "d"),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseAdded("id", "pId", "i", string.Empty, 1, 1, 0, "d"),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseAdded("id", "pId", "i", "c", 0, 1, 0, "d"),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseAdded("id", "pId", "i", "c", 1, -1, 0, "d"),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseAdded("id", "pId", "i", "c", 1, 1, -1, "d"),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseDetailsChanged("id", "pId", string.Empty, "c", 1, 1, 0, "d"),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseDetailsChanged("id", "pId", "i", string.Empty, 1, 1, 0, "d"),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseDetailsChanged("id", "pId", "i", "c", 0, 1, 0, "d"),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseDetailsChanged("id", "pId", "i", "c", 1, -1, 0, "d"),
      }
    ],
    [
      new Fact[]
      {
        new ReceiptCreated("id", "item", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
        new PurchaseDetailsChanged("id", "pId", "i", "c", 1, 1, -1, "d"),
      }
    ]
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
