namespace ExpenseExplorer.Application.Tests.TestData;

using System.Collections;
using ExpenseExplorer.Application.Receipts.Commands;

public sealed class ValidUpdatePurchaseDetailsCommandData : IEnumerable<object[]>
{
  private const string _rId = "receiptWithPurchaseId";
  private const string _pId = "purchaseId";

  private static readonly List<object[]> _data =
  [
    [new UpdatePurchaseDetailsCommand(_rId, _pId, null, null, null, null, null, null)],
    [new UpdatePurchaseDetailsCommand(_rId, _pId, "i", null, null, null, null, null)],
    [new UpdatePurchaseDetailsCommand(_rId, _pId, null, "c", null, null, null, null)],
    [new UpdatePurchaseDetailsCommand(_rId, _pId, null, null, 5, null, null, null)],
    [new UpdatePurchaseDetailsCommand(_rId, _pId, null, null, null, 5, null, null)],
    [new UpdatePurchaseDetailsCommand(_rId, _pId, null, null, null, null, 1, null)],
    [new UpdatePurchaseDetailsCommand(_rId, _pId, null, null, null, null, null, "desc")],
    [new UpdatePurchaseDetailsCommand(_rId, _pId, null, null, null, null, null, string.Empty)],
    [new UpdatePurchaseDetailsCommand(_rId, _pId, "i", "c", 5, 5, 1, "desc")],
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
