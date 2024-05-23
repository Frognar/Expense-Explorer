namespace ExpenseExplorer.ReadModel;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CommandHub;
using CommandHub.Commands;
using EventStore.Client;
using ExpenseExplorer.ReadModel.Commands;
using ExpenseExplorer.ReadModel.Facts;
using ExpenseExplorer.ReadModel.Models.Persistence;
using FunctionalCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by DI container")]
internal sealed class FactProcessor(
  string connectionString,
  string postgresConnectionString,
  ISender sender) : BackgroundService
{
  private readonly EventStoreClient _client = new(EventStoreClientSettings.Create(connectionString));
  private readonly ExpenseExplorerContext _context = new(postgresConnectionString);
  private readonly ISender _sender = sender;

  public override void Dispose()
  {
    _client.Dispose();
    _context.Dispose();
    base.Dispose();
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    Position lastProcessedPosition = await GetLastProcessedPositionAsync(stoppingToken);
    EventStoreClient.StreamSubscriptionResult result =
      _client.SubscribeToAll(FromAll.After(lastProcessedPosition), cancellationToken: stoppingToken);

    await foreach (ResolvedEvent resolvedEvent in result)
    {
      Task task = resolvedEvent.Event.EventType switch
      {
        FactTypes.ReceiptCreatedFactType => HandleAsync<CreateReceiptCommand>(resolvedEvent, stoppingToken),
        FactTypes.StoreCorrectedFactType => HandleAsync<CorrectStoreCommand>(resolvedEvent, stoppingToken),
        FactTypes.PurchaseDateChangedFactType => HandleAsync<ChangePurchaseDateCommand>(resolvedEvent, stoppingToken),
        FactTypes.PurchaseAddedFactType => HandleAsync<AddPurchaseCommand>(resolvedEvent, stoppingToken),
        FactTypes.PurchaseDetailsChangedFactType
          => HandleAsync<UpdatePurchaseDetailsCommand>(resolvedEvent, stoppingToken),
        FactTypes.PurchaseRemovedFactType => HandleAsync<RemovePurchaseCommand>(resolvedEvent, stoppingToken),
        FactTypes.ReceiptDeletedFactType => HandleAsync<DeleteReceiptCommand>(resolvedEvent, stoppingToken),
        FactTypes.IncomeCreatedFactType => HandleAsync<AddIncomeCommand>(resolvedEvent, stoppingToken),
        FactTypes.IncomeSourceCorrectedFactType
          => HandleAsync<CorrectIncomeSourceCommand>(resolvedEvent, stoppingToken),
        FactTypes.IncomeAmountCorrectedFactType
          => HandleAsync<CorrectIncomeAmountCommand>(resolvedEvent, stoppingToken),
        FactTypes.IncomeCategoryCorrectedFactType
          => HandleAsync<CorrectIncomeCategoryCommand>(resolvedEvent, stoppingToken),
        FactTypes.IncomeReceivedDateCorrectedFactType
          => HandleAsync<CorrectIncomeReceivedDateCommand>(resolvedEvent, stoppingToken),
        _ => Task.FromResult(() => Console.WriteLine(resolvedEvent.Event.EventType)),
      };

      await task;
      lastProcessedPosition = resolvedEvent.Event.Position;
      await SavePositionAsync(lastProcessedPosition, stoppingToken);
    }
  }

  private async Task<Position> GetLastProcessedPositionAsync(CancellationToken cancellationToken)
  {
    Position position = await _context.Positions
      .OrderByDescending(p => p.CommitPosition)
      .Select(p => new Position(p.CommitPosition, p.PreparePosition))
      .FirstOrDefaultAsync(cancellationToken);

    return position == default ? Position.Start : position;
  }

  private async Task SavePositionAsync(Position position, CancellationToken cancellationToken)
  {
    _context.Positions.Add(new DbPosition(position.CommitPosition, position.PreparePosition));
    await _context.SaveChangesAsync(cancellationToken);
  }

  private async Task HandleAsync<TCommand>(ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
    where TCommand : ICommand<Unit>
  {
    string jsonFact = System.Text.Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
    TCommand command = JsonSerializer.Deserialize<TCommand>(jsonFact)!;
    await _sender.SendAsync(command, cancellationToken);
  }
}
