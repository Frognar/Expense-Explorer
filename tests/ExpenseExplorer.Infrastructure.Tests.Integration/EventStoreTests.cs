namespace ExpenseExplorer.Infrastructure.Tests.Integration;

using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using FluentAssertions;

public class EventStoreTests : BaseIntegrationTest
{
  [Fact]
  public async Task FailsWhenConcurrentWriteOccurs()
  {
    Receipt receipt = await CreateReceipt();
    Receipt receipt1 = receipt.CorrectStore(Store.Create("s1"));
    Receipt receipt2 = receipt.CorrectStore(Store.Create("s2"));

    var result1 = await ReceiptRepository.SaveAsync(receipt1, default);
    var result2 = await ReceiptRepository.SaveAsync(receipt2, default);

    result1.Match(_ => false, _ => true).Should().BeTrue();
    result2.Match(_ => false, _ => true).Should().BeFalse();
  }

  private async Task<Receipt> CreateReceipt()
  {
    DateOnly today = DateOnly.MinValue;
    var receipt = Receipt.New(Store.Create("s"), PurchaseDate.Create(today, today), today);
    var result = await ReceiptRepository.SaveAsync(receipt, default);
    var version = result.Match(_ => Version.New(), v => v);
    return receipt.WithVersion(version);
  }
}
