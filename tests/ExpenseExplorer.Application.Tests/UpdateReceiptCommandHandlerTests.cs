namespace ExpenseExplorer.Application.Tests;

using System.Diagnostics;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Tests.Common.Generators.Commands;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public class UpdateReceiptCommandHandlerTests
{
  private const string _originalStoreName = "store";
  private static readonly DateOnly _originalPurchaseDate = new(2000, 1, 1);
  private readonly FakeReceiptRepository _receiptRepository = new();

  [Theory]
  [InlineData(null, null)]
  [InlineData("new store", null)]
  [InlineData(null, 100000L)]
  [InlineData("new store", 100000L)]
  public async Task UpdatesValuesOnReceiptWhenValidCommand(string? storeName, long? ticks)
  {
    DateOnly? data = ticks.HasValue ? DateOnly.FromDateTime(new DateTime(ticks.Value, DateTimeKind.Utc)) : null;
    UpdateReceiptCommand command = new("receiptId", storeName, data, data ?? DateOnly.MaxValue);

    Receipt receipt = await HandleValid(command);

    receipt.Store.Name.Should().Be(storeName ?? _originalStoreName);
    receipt.PurchaseDate.Date.Should().Be(data ?? _originalPurchaseDate);
    receipt.Version.Value.Should().Be((storeName is null ? 0UL : 1UL) + (ticks is null ? 0UL : 1UL));
  }

  [Fact]
  public async Task SavesReceiptWhenValidCommand()
  {
    DateOnly today = new(2024, 1, 1);

    Receipt receipt = await HandleValid(new UpdateReceiptCommand("receiptId", "new store", today, today));

    _receiptRepository.Should()
      .Contain(r => r.Id == receipt.Id && r.PurchaseDate.Date == today && r.Store.Name == "new store");
  }

  [Property(Arbitrary = [typeof(ValidUpdateReceiptCommandGenerator)])]
  public async Task ReturnsNotFoundFailureWhenReceiptNotFound(UpdateReceiptCommand command)
  {
    Failure failure = await HandleInvalid(command with { ReceiptId = "invalid-Id" });
    failure.Should().BeOfType<NotFoundFailure>();
  }

  [Property(Arbitrary = [typeof(InvalidUpdateReceiptCommandGenerator)])]
  public async Task ReturnsValidationFailureWhenRequestIsInvalid(UpdateReceiptCommand command)
  {
    Failure failure = await HandleInvalid(command);
    failure.Should().BeOfType<ValidationFailure>();
  }

  private async Task<Failure> HandleInvalid(UpdateReceiptCommand command)
    => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Receipt> HandleValid(UpdateReceiptCommand command)
    => (await Handle(command)).Match(_ => throw new UnreachableException(), r => r);

  private async Task<Result<Receipt>> Handle(UpdateReceiptCommand command)
    => await new UpdateReceiptCommandHandler(_receiptRepository).HandleAsync(command);
}
