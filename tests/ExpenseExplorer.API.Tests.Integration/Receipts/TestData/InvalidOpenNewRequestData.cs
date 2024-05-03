namespace ExpenseExplorer.API.Tests.Integration.Receipts.TestData;

public class InvalidOpenNewRequestData : IEnumerable<object[]>
{
  private static readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Today);

  private static readonly List<object[]> _data =
  [
    [new { storeName = string.Empty, purchaseDate = _today }],
    [new { storeName = "store", purchaseDate = _today.AddDays(1) }],
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
