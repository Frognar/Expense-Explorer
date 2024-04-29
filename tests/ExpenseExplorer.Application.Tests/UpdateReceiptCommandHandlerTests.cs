namespace ExpenseExplorer.Application.Tests;

using System.Collections.ObjectModel;
using System.Diagnostics;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;
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
    Receipt s = await HandleValid(command);
    s.Store.Name.Should().Be(storeName ?? _originalStoreName);
    s.PurchaseDate.Date.Should().Be(data ?? _originalPurchaseDate);
    s.Version.Value.Should().Be(1UL + (storeName is null ? 0UL : 1UL) + (ticks is null ? 0UL : 1UL));
  }

  [Property(Arbitrary = [typeof(ValidUpdateReceiptCommandGenerator)])]
  public async Task ReturnsNotFoundFailureWhenReceiptNotFound(UpdateReceiptCommand command)
  {
    Result<Receipt> result = await Handle(command with { ReceiptId = "invalid-Id" });
    Failure failure = result.Match(e => e, _ => throw new UnreachableException());
    failure.Should().BeOfType<NotFoundFailure>();
  }

  [Property(Arbitrary = [typeof(InvalidUpdateReceiptCommandGenerator)])]
  public async Task ReturnsValidationFailureWhenRequestIsInvalid(UpdateReceiptCommand command)
  {
    Result<Receipt> result = await Handle(command);
    Failure failure = result.Match(e => e, _ => throw new UnreachableException());
    failure.Should().BeOfType<ValidationFailure>();
  }

  private async Task<Receipt> HandleValid(UpdateReceiptCommand command)
  {
    Result<Receipt> result = await Handle(command);
    return result.Match(_ => throw new UnreachableException(), r => r);
  }

  private async Task<Result<Receipt>> Handle(UpdateReceiptCommand command)
  {
    UpdateReceiptCommandHandler handler = new(_receiptRepository);
    return await handler.HandleAsync(command);
  }

  private sealed class FakeReceiptRepository : Collection<Receipt>, IReceiptRepository
  {
    public FakeReceiptRepository()
    {
      Fact fact = new ReceiptCreated("receiptId", _originalStoreName, _originalPurchaseDate, _originalPurchaseDate);
      Add(Receipt.Recreate([fact], Version.Create(1UL)));
    }

    public Task<Result<Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
    {
      Version version = Version.Create(receipt.Version.Value + (ulong)receipt.UnsavedChanges.Count());
      this[0] = receipt.WithVersion(version).ClearChanges();
      return Task.FromResult(Success.From(version));
    }

    public Task<Result<Receipt>> GetAsync(Id id, CancellationToken cancellationToken)
    {
      Receipt? receipt = this.SingleOrDefault(r => r.Id == id);
      return receipt is null
        ? Task.FromResult(Fail.OfType<Receipt>(Failure.NotFound("Receipt not found", id.Value)))
        : Task.FromResult(Success.From(receipt));
    }
  }
}
