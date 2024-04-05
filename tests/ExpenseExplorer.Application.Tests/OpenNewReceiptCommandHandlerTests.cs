namespace ExpenseExplorer.Application.Tests;

using System.Collections.ObjectModel;
using System.Diagnostics;
using ExpenseExplorer.Application.Errors;
using ExpenseExplorer.Application.Monads;
using ExpenseExplorer.Application.Receipts.Commands;
using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public class OpenNewReceiptCommandHandlerTests
{
  private readonly FakeFactBus _factBus = new();
  private readonly FakeReceiptRepository _receiptRepository = new();

  [Property(Arbitrary = [typeof(ValidOpenNewReceiptCommandGenerator)])]
  public async Task CanHandleValidCommand(OpenNewReceiptCommand command)
  {
    var receipt = (await Handle(command))
      .Match(_ => throw new UnreachableException(), receipt => receipt);

    receipt.Store.Name.Should().Be(command.StoreName.Trim());
    receipt.PurchaseDate.Date.Should().Be(command.PurchaseDate);
    receipt.Version.Value.Should().Be(0);
  }

  [Property(Arbitrary = [typeof(ValidOpenNewReceiptCommandGenerator)])]
  public async Task PublishesReceiptCreatedWhenValidCommand(OpenNewReceiptCommand command)
  {
    var receipt = (await Handle(command))
      .Match(_ => throw new UnreachableException(), receipt => receipt);

    _factBus.Should()
      .ContainEquivalentOf(new ReceiptCreated(receipt.Id, receipt.Store, receipt.PurchaseDate, TodayDateOnly));
  }

  [Property(Arbitrary = [typeof(InvalidOpenNewReceiptCommandGenerator)])]
  public async Task CanHandleInvalidCommand(OpenNewReceiptCommand command)
  {
    var failure = (await Handle(command))
      .Match(failure => failure, _ => throw new UnreachableException());

    failure.Should().BeOfType<ValidationFailure>();
  }

  [Property(Arbitrary = [typeof(ValidOpenNewReceiptCommandGenerator)])]
  public async Task SavesReceiptWhenValidCommand(OpenNewReceiptCommand command)
  {
    var receipt = (await Handle(command))
      .Match(_ => throw new UnreachableException(), receipt => receipt);

    _receiptRepository.Should().Contain(r => r.Id == receipt.Id);
  }

  private async Task<Either<Failure, Receipt>> Handle(OpenNewReceiptCommand command)
  {
    OpenNewReceiptCommandHandler handler = new(_receiptRepository, _factBus);
    return await handler.HandleAsync(command);
  }

  private sealed class FakeReceiptRepository : Collection<Receipt>, IReceiptRepository
  {
    public Task<Either<Failure, Version>> SaveAsync(Receipt receipt, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Add(receipt);
      return Task.FromResult(Right.From<Failure, Version>(Version.Create(receipt.Version.Value + 1)));
    }

    public Task<Either<Failure, Receipt>> GetAsync(Id id, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      return Task.FromResult(Left.From<Failure, Receipt>(new NotFoundFailure("Receipt not found", id)));
    }
  }

  private sealed class FakeFactBus : Collection<Fact>, IFactBus
  {
    public Task PublishAsync<T>(T fact, CancellationToken cancellationToken = default)
      where T : Fact
    {
      Add(fact);
      return Task.CompletedTask;
    }
  }
}
