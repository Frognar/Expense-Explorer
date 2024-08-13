using System.Collections;
using ExpenseExplorer.Domain.Incomes.Facts;

namespace ExpenseExplorer.Domain.Tests.TestData;

public sealed class IncomeCorruptedFactsForRecreate : IEnumerable<object[]>
{
  private static readonly DateOnly _today = new DateOnly(2000, 1, 1);

  private static readonly List<object[]> _data =
  [
    [new Fact[] { new IncomeCreated(string.Empty, "s", 1, _today, "c", "d", _today) }],
    [new Fact[] { new IncomeCreated("id", string.Empty, 1, _today, "c", "d", _today) }],
    [new Fact[] { new IncomeCreated("id", "s", -1, _today, "c", "d", _today) }],
    [new Fact[] { new IncomeCreated("id", "s", 1, _today.AddDays(1), "c", "d", _today) }],
    [new Fact[] { new IncomeCreated("id", "s", 1, _today, string.Empty, "d", _today) }],
    [
      new Fact[] { new IncomeCreated("id", "s", 1, _today, "c", "d", _today), new SourceCorrected("id", string.Empty), }
    ],
    [
      new Fact[] { new IncomeCreated("id", "s", 1, _today, "c", "d", _today), new AmountCorrected("id", -1), }
    ],
    [
      new Fact[] { new IncomeCreated("id", "s", 1, _today, "c", "d", _today), new CategoryCorrected("id", string.Empty), }
    ],
    [
      new Fact[] { new IncomeCreated("id", "s", 1, _today, "c", "d", _today), new ReceivedDateCorrected("id", _today.AddDays(1), _today), }
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
