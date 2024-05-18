namespace ExpenseExplorer.Application.Tests.TestData;

public sealed class ValidUpdateIncomeDetailsCommandData : IEnumerable<object[]>
{
  private const string _id = "incomeId";

  private static readonly List<object[]> _data =
  [
    [new UpdateIncomeDetailsCommand(_id, null, null, null, null, null, DateOnly.MaxValue)],
    [new UpdateIncomeDetailsCommand(_id, "new source", null, null, null, null, DateOnly.MaxValue)],
    [new UpdateIncomeDetailsCommand(_id, null, 5, null, null, null, DateOnly.MaxValue)],
    [new UpdateIncomeDetailsCommand(_id, null, null, "new category", null, null, DateOnly.MaxValue)],
    [new UpdateIncomeDetailsCommand(_id, null, null, null, new DateOnly(2024, 1, 1), null, DateOnly.MaxValue)],
    [new UpdateIncomeDetailsCommand(_id, null, null, null, null, "new description", DateOnly.MaxValue)],
    [new UpdateIncomeDetailsCommand(_id, "new source", 5, "new category", new DateOnly(2024, 1, 1), "new description", DateOnly.MaxValue)],
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
