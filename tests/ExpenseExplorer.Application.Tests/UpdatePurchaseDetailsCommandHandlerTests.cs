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
    purchase.Item.Name.Should().Be(command.Item?.Trim() ?? purchase.Item.Name);
    purchase.Category.Name.Should().Be(command.Category?.Trim() ?? purchase.Category.Name);
    purchase.Quantity.Value.Should().Be(Math.Round(command.Quantity ?? purchase.Quantity.Value, Quantity.Precision));
    purchase.UnitPrice.Value.Should().Be(Math.Round(command.UnitPrice ?? purchase.UnitPrice.Value, Money.Precision));
    purchase.TotalDiscount.Value.Should()
      .Be(Math.Round(command.TotalDiscount ?? purchase.TotalDiscount.Value, Money.Precision));

    purchase.Description.Value.Should().Be(command.Description?.Trim() ?? purchase.Description.Value);
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
