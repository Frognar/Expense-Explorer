namespace ExpenseExplorer.Application.Tests.TestData;

using System.Collections;
using ExpenseExplorer.Application.Receipts.Commands;

public sealed class ValidUpdateReceiptCommandData : IEnumerable<object[]>
{
  private const string _rId = "receiptId";

  private static readonly List<object[]> _data =
  [
    [new UpdateReceiptCommand(_rId, null, null, DateOnly.MaxValue)],
    [new UpdateReceiptCommand(_rId, "new store", null, DateOnly.MaxValue)],
    [new UpdateReceiptCommand(_rId, null, new DateOnly(2020, 1, 1), DateOnly.MaxValue)],
    [new UpdateReceiptCommand(_rId, "new store", new DateOnly(2020, 1, 1), DateOnly.MaxValue)],
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
