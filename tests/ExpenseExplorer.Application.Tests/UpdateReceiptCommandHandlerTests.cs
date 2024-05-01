namespace ExpenseExplorer.Application.Tests;

using System.Diagnostics;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Tests.TestData;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.Commands;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public class UpdateReceiptCommandHandlerTests
{
  private const string _originalStoreName = "store";
  private static readonly DateOnly _originalPurchaseDate = new(2000, 1, 1);

  private readonly FakeReceiptRepository _receiptRepository =
  [
    Receipt.Recreate(
      [new ReceiptCreated("receiptId", _originalStoreName, _originalPurchaseDate, _originalPurchaseDate)],
      Version.Create(0UL))
  ];

  [Theory]
  [ClassData(typeof(ValidUpdateReceiptCommandData))]
  public async Task UpdatesValuesOnReceiptWhenValidCommand(UpdateReceiptCommand command)
  {
    Receipt receipt = await HandleValid(command);

    receipt.Store.Name.Should().Be(command.StoreName ?? _originalStoreName);
    receipt.PurchaseDate.Date.Should().Be(command.PurchaseDate ?? _originalPurchaseDate);
    receipt.Version.Value.Should()
      .Be((command.StoreName is null ? 0UL : 1UL) + (command.PurchaseDate is null ? 0UL : 1UL));
  }

  [Fact]
  public async Task SavesReceiptWhenValidCommand()
  {
    DateOnly today = new(2024, 1, 1);

    Receipt receipt = await HandleValid(new UpdateReceiptCommand("receiptId", "new store", today, today));

    _receiptRepository.Should()
      .Contain(
        r => r.Id == receipt.Id
             && r.PurchaseDate.Date == today
             && r.Store.Name == "new store"
             && r.Version.Value == 2UL);
  }

  [Property(Arbitrary = [typeof(ValidUpdateReceiptCommandGenerator)])]
  public async Task ReturnsNotFoundFailureWhenReceiptNotFound(UpdateReceiptCommand command)
  {
    Failure failure = await HandleInvalid(command with { ReceiptId = "invalid-Id" });
    failure.Should().BeOfType<NotFoundFailure>();
  }

  [Property(Arbitrary = [typeof(InvalidUpdateReceiptCommandGenerator)])]
  public async Task ReturnsValidationFailureWhenInvalidCommand(UpdateReceiptCommand command)
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
