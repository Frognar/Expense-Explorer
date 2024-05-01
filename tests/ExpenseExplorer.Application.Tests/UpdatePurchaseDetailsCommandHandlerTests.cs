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

    receipt.Id.Value.Should().Be(command.ReceiptId);
    Purchase purchase = receipt.Purchases.Single(p => p.Id == Id.Create(command.PurchaseId));
    AssertPurchase(
      purchase,
      command.Item?.Trim(),
      command.Category?.Trim(),
      command.Quantity.HasValue ? Math.Round(command.Quantity.Value, Quantity.Precision) : null,
      command.UnitPrice.HasValue ? Math.Round(command.UnitPrice.Value, Money.Precision) : null,
      command.TotalDiscount.HasValue ? Math.Round(command.TotalDiscount.Value, Money.Precision) : null,
      command.Description?.Trim() ?? purchase.Description.Value);
  }

  [Fact]
  public async Task SavesReceiptWhenValidCommand()
  {
    UpdatePurchaseDetailsCommand command = new("receiptWithPurchaseId", "purchaseId", "i", "c", 1, 1, 0, "d");

    _ = await HandleValid(command);

    Receipt receipt = _receiptRepository.Single(r => r.Id.Value == "receiptWithPurchaseId");
    receipt.Version.Value.Should().Be(2UL);
    Purchase purchase = receipt.Purchases.Single(p => p.Id == Id.Create(command.PurchaseId));
    AssertPurchase(purchase, "i", "c", 1, 1, 0, "d");
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

  private static void AssertPurchase(
    Purchase purchase,
    string? expectedItem,
    string? expectedCategory,
    decimal? expectedQuantity,
    decimal? expectedUnitPrice,
    decimal? expectedTotalDiscount,
    string? expectedDescription)
  {
    if (expectedItem is not null)
    {
      purchase.Item.Name.Should().Be(expectedItem);
    }

    if (expectedCategory is not null)
    {
      purchase.Category.Name.Should().Be(expectedCategory);
    }

    if (expectedQuantity is not null)
    {
      purchase.Quantity.Value.Should().Be(expectedQuantity);
    }

    if (expectedUnitPrice is not null)
    {
      purchase.UnitPrice.Value.Should().Be(expectedUnitPrice);
    }

    if (expectedTotalDiscount is not null)
    {
      purchase.TotalDiscount.Value.Should().Be(expectedTotalDiscount);
    }

    if (expectedDescription is not null)
    {
      purchase.Description.Value.Should().Be(expectedDescription);
    }

    if (expectedItem is not null)
    {
      purchase.Item.Name.Should().Be(expectedItem);
    }
  }

  private async Task<Failure> HandleInvalid(UpdatePurchaseDetailsCommand command)
    => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Receipt> HandleValid(UpdatePurchaseDetailsCommand command)
    => (await Handle(command)).Match(_ => throw new UnreachableException(), r => r);

  private async Task<Result<Receipt>> Handle(UpdatePurchaseDetailsCommand command)
    => await new UpdatePurchaseDetailsCommandHandler(_receiptRepository).HandleAsync(command);
}
