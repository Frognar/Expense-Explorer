using ExpenseExplorer.Tests.Common;

namespace ExpenseExplorer.Application.Tests;

public class UpdatePurchaseDetailsCommandHandlerTests
{
  private const string _originalItem = "item";
  private const string _originalCategory = "category";
  private const decimal _originalQuantity = 2;
  private const decimal _originalUnitPrice = 2;
  private const decimal _originalTotalDiscount = 2;
  private const string _originalDescription = "description";

  private readonly FakeReceiptRepository _receiptRepository =
  [
    Receipt.Recreate(
        [
          new ReceiptCreated("receiptWithPurchaseId", "store", new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)),
          new PurchaseAdded(
            "receiptWithPurchaseId",
            "purchaseId",
            _originalItem,
            _originalCategory,
            _originalQuantity,
            _originalUnitPrice,
            _originalTotalDiscount,
            _originalDescription)
        ],
        Version.Create(1UL))
      .Match(_ => throw new UnreachableException(), r => r)
  ];

  [Theory]
  [ClassData(typeof(ValidUpdatePurchaseDetailsCommandData))]
  public async Task UpdatesPurchaseDetailsWhenCommandIsValid(UpdatePurchaseDetailsCommand command)
  {
    Receipt receipt = await HandleValid(command);

    receipt.Id.Value.Should().Be(command.ReceiptId);
    Purchase purchase = receipt.Purchases.Single(p => p.Id.Value == command.PurchaseId);
    AssertPurchase(
      purchase,
      command.Item?.Trim() ?? _originalItem,
      command.Category?.Trim() ?? _originalCategory,
      Math.Round(command.Quantity ?? _originalQuantity, Quantity.Precision),
      Math.Round(command.UnitPrice ?? _originalUnitPrice, Money.Precision),
      Math.Round(command.TotalDiscount ?? _originalTotalDiscount, Money.Precision),
      command.Description?.Trim() ?? _originalDescription);
  }

  [Fact]
  public async Task SavesReceiptWhenValidCommand()
  {
    UpdatePurchaseDetailsCommand command = new("receiptWithPurchaseId", "purchaseId", "i", "c", 1, 1, 0, "d");

    _ = await HandleValid(command);

    Receipt receipt = _receiptRepository.Single(r => r.Id.Value == "receiptWithPurchaseId");
    receipt.Version.Value.Should().Be(2UL);
    Purchase purchase = receipt.Purchases.Single(p => p.Id.Value == command.PurchaseId);
    AssertPurchase(purchase, "i", "c", 1, 1, 0, "d");
  }

  [Property(Arbitrary = [typeof(ValidUpdatePurchaseDetailsCommandGenerator)])]
  public async Task ReturnsFailureWhenReceiptNotFound(UpdatePurchaseDetailsCommand command)
  {
    Failure failure = await HandleInvalid(command with { ReceiptId = "invalid-Id" });
    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, id) => id.Should().Be("invalid-Id"),
      (_, _) => throw new InvalidOperationException("Unexpected validation failure"));
  }

  [Property(Arbitrary = [typeof(ValidUpdatePurchaseDetailsCommandGenerator)])]
  public async Task ReturnsFailureWhenPurchaseNotFound(UpdatePurchaseDetailsCommand command)
  {
    Failure failure = await HandleInvalid(command with { PurchaseId = "invalid-Id" });
    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, id) => id.Should().Be("invalid-Id"),
      (_, _) => throw new InvalidOperationException("Unexpected validation failure"));
  }

  [Property(Arbitrary = [typeof(InvalidUpdatePurchaseDetailsCommandGenerator)])]
  public async Task ReturnsFailureWhenInvalidCommand(UpdatePurchaseDetailsCommand command)
  {
    Failure failure = await HandleInvalid(command);
    failure.Match(
      (_, _) => throw new InvalidOperationException("Unexpected fatal failure"),
      (_, _) => throw new InvalidOperationException("Unexpected not found failure"),
      (_, errors) => errors.Should().NotBeEmpty());
  }

  private static void AssertPurchase(
    Purchase purchase,
    string expectedItem,
    string expectedCategory,
    decimal expectedQuantity,
    decimal expectedUnitPrice,
    decimal expectedTotalDiscount,
    string expectedDescription)
  {
    purchase.Item.Name.Should().Be(expectedItem);
    purchase.Category.Name.Should().Be(expectedCategory);
    purchase.Quantity.Value.Should().Be(expectedQuantity);
    purchase.UnitPrice.Value.Should().Be(expectedUnitPrice);
    purchase.TotalDiscount.Value.Should().Be(expectedTotalDiscount);
    purchase.Description.Value.Should().Be(expectedDescription);
    purchase.Item.Name.Should().Be(expectedItem);
  }

  private async Task<Failure> HandleInvalid(UpdatePurchaseDetailsCommand command)
    => (await Handle(command)).Match(f => f, _ => throw new UnreachableException());

  private async Task<Receipt> HandleValid(UpdatePurchaseDetailsCommand command)
    => (await Handle(command)).Match(_ => throw new UnreachableException(), r => r);

  private async Task<Result<Receipt>> Handle(UpdatePurchaseDetailsCommand command)
    => await new UpdatePurchaseDetailsCommandHandler(_receiptRepository).HandleAsync(command);
}
