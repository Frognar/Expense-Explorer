namespace ExpenseExplorer.Application.Tests;

using System.Diagnostics;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.Commands;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public class UpdatePurchaseDetailsCommandHandlerTests
{
  private readonly FakeReceiptRepository _receiptRepository = new();

  [Property(Arbitrary = [typeof(ValidUpdatePurchaseDetailsCommandGenerator)])]
  public async Task UpdatesPurchaseDetailsWhenCommandIsValid(UpdatePurchaseDetailsCommand command)
  {
    Receipt receipt = await HandleValid(command);

    receipt.Id.Value
      .Should()
      .Be(command.ReceiptId);

    Purchase purchase = receipt.Purchases
      .Single(p => p.Id == Id.Create(command.PurchaseId));

    purchase.Item.Name
      .Should()
      .Be(command.Item?.Trim() ?? purchase.Item.Name);

    purchase.Category.Name
      .Should()
      .Be(command.Category?.Trim() ?? purchase.Category.Name);

    purchase.Quantity.Value
      .Should()
      .Be(Math.Round(command.Quantity ?? purchase.Quantity.Value, Quantity.Precision));

    purchase.UnitPrice.Value
      .Should()
      .Be(Math.Round(command.UnitPrice ?? purchase.UnitPrice.Value, Money.Precision));

    purchase.TotalDiscount.Value
      .Should()
      .Be(Math.Round(command.TotalDiscount ?? purchase.TotalDiscount.Value, Money.Precision));

    purchase.Description.Value
      .Should()
      .Be(command.Description?.Trim() ?? purchase.Description.Value);
  }

  [Fact]
  public async Task SavesReceiptWhenValidCommand()
  {
    UpdatePurchaseDetailsCommand command = new("receiptWithPurchaseId", "purchaseId", "i", "c", 1, 1, 0, "d");

    _ = await HandleValid(command);

    Receipt receipt = _receiptRepository.Single(r => r.Id.Value == "receiptWithPurchaseId");
    receipt.Version.Value.Should().Be(2UL);
    Purchase purchase = receipt.Purchases.Single(p => p.Id == Id.Create(command.PurchaseId));
    purchase.Item.Name.Should().Be("i");
    purchase.Category.Name.Should().Be("c");
    purchase.Quantity.Value.Should().Be(1);
    purchase.UnitPrice.Value.Should().Be(1);
    purchase.TotalDiscount.Value.Should().Be(0);
    purchase.Description.Value.Should().Be("d");
  }

  [Property(Arbitrary = [typeof(ValidUpdatePurchaseDetailsCommandGenerator)])]
  public async Task ReturnsFailureWhenReceiptNotFound(UpdatePurchaseDetailsCommand command)
  {
    Failure failure = await HandleInvalid(command with { ReceiptId = "invalid-Id" });
    failure.Should().BeOfType<NotFoundFailure>();
  }

  [Property(Arbitrary = [typeof(ValidUpdatePurchaseDetailsCommandGenerator)])]
  public async Task ReturnsFailureWhenPurchaseNotFound(UpdatePurchaseDetailsCommand command)
  {
    Failure failure = await HandleInvalid(command with { PurchaseId = "invalid-Id" });
    failure.Should().BeOfType<NotFoundFailure>();
  }

  private async Task<Failure> HandleInvalid(UpdatePurchaseDetailsCommand command)
    => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Receipt> HandleValid(UpdatePurchaseDetailsCommand command)
    => (await Handle(command)).Match(_ => throw new UnreachableException(), r => r);

  private async Task<Result<Receipt>> Handle(UpdatePurchaseDetailsCommand command)
    => await new UpdatePurchaseDetailsCommandHandler(_receiptRepository).HandleAsync(command);
}
