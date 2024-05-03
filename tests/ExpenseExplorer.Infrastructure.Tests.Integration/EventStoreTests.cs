namespace ExpenseExplorer.Infrastructure.Tests.Integration;

using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common;
using FluentAssertions;
using FunctionalCore.Monads;

public class EventStoreTests : BaseIntegrationTest
{
  [Fact]
  public async Task FailsWhenConcurrentWriteOccurs()
  {
    Receipt receipt = await CreateReceipt();
    Receipt receipt1 = receipt.CorrectStore(TestFactory.Store("s1"));
    Receipt receipt2 = receipt.CorrectStore(TestFactory.Store("s2"));

    Result<Version> result1 = await ReceiptRepository.SaveAsync(receipt1, default);
    Result<Version> result2 = await ReceiptRepository.SaveAsync(receipt2, default);

    result1.Match(_ => false, _ => true).Should().BeTrue();
    result2.Match(_ => false, _ => true).Should().BeFalse();
  }

  private async Task<Receipt> CreateReceipt()
  {
    Receipt receipt = TestFactory.Receipt("s", new DateOnly(2000, 1, 1));
    Result<Version> result = await ReceiptRepository.SaveAsync(receipt, default);
    Version version = result.Match(_ => Version.New(), v => v);
    return receipt.WithVersion(version);
  }
}
