namespace ExpenseExplorer.ReadModel;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CommandHub;
using EventStore.Client;
using ExpenseExplorer.ReadModel.Commands;
using ExpenseExplorer.ReadModel.Facts;
using Microsoft.Extensions.Hosting;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by DI container")]
internal sealed class FactProcessor(string connectionString, ISender sender) : BackgroundService
{
  private const string _file = "lastProcessedPosition.txt";
  private readonly EventStoreClient _client = new(EventStoreClientSettings.Create(connectionString));
  private readonly ISender _sender = sender;

  public override void Dispose()
  {
    _client.Dispose();
    base.Dispose();
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    Position lastProcessedPosition = await GetLastProcessedPositionAsync();
    EventStoreClient.StreamSubscriptionResult result = _client.SubscribeToAll(
      FromAll.After(lastProcessedPosition),
      cancellationToken: stoppingToken);

    await foreach (ResolvedEvent resolvedEvent in result)
    {
      Task task = resolvedEvent.Event.EventType switch
      {
        FactTypes.ReceiptCreatedFactType => HandleReceiptCreationAsync(resolvedEvent, stoppingToken),
        FactTypes.PurchaseAddedFactType => HandlePurchaseAdditionAsync(resolvedEvent, stoppingToken),
        FactTypes.PurchaseDetailsChangedFactType => HandlePurchaseDetailsUpdatedAsync(resolvedEvent, stoppingToken),
        _ => Task.FromResult(() => Console.WriteLine(resolvedEvent.Event.EventType)),
      };

      await task;
      lastProcessedPosition = resolvedEvent.Event.Position;
      await File.WriteAllTextAsync(_file, lastProcessedPosition.ToString(), stoppingToken);
    }
  }

  private static async Task<Position> GetLastProcessedPositionAsync()
    => FileWithPositionExists()
      ? await ReadPositionFromFileAsync()
      : Position.Start;

  private static bool FileWithPositionExists() => File.Exists(_file);

  private static async Task<Position> ReadPositionFromFileAsync()
    => Position.TryParse(await File.ReadAllTextAsync(_file), out Position? position)
      ? position ?? Position.Start
      : Position.Start;

  private async Task HandleReceiptCreationAsync(ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
  {
    string jsonFact = System.Text.Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
    CreateReceiptCommand command = JsonSerializer.Deserialize<CreateReceiptCommand>(jsonFact)!;
    await _sender.SendAsync(command, cancellationToken);
  }

  private async Task HandlePurchaseAdditionAsync(ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
  {
    string jsonFact = System.Text.Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
    AddPurchaseCommand command = JsonSerializer.Deserialize<AddPurchaseCommand>(jsonFact)!;
    await _sender.SendAsync(command, cancellationToken);
  }

  private async Task HandlePurchaseDetailsUpdatedAsync(ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
  {
    string jsonFact = System.Text.Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
    UpdatePurchaseDetailsCommand command = JsonSerializer.Deserialize<UpdatePurchaseDetailsCommand>(jsonFact)!;
    await _sender.SendAsync(command, cancellationToken);
  }
}
