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
  private Position _lastProcessedPosition = GetLastProcessedPosition();

  public override void Dispose()
  {
    _client.Dispose();
    base.Dispose();
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    EventStoreClient.StreamSubscriptionResult result = _client.SubscribeToAll(
      FromAll.After(_lastProcessedPosition),
      cancellationToken: stoppingToken);

    await foreach (ResolvedEvent resolvedEvent in result.WithCancellation(stoppingToken))
    {
      Task task = resolvedEvent.Event.EventType switch
      {
        FactTypes.ReceiptCreatedFactType => HandleReceiptCreationAsync(resolvedEvent, stoppingToken),
        _ => Task.FromResult(() => Console.WriteLine(resolvedEvent.Event.EventType)),
      };

      await task;
      _lastProcessedPosition = resolvedEvent.Event.Position;
      await File.WriteAllTextAsync(_file, _lastProcessedPosition.ToString(), stoppingToken);
    }
  }

  private static Position GetLastProcessedPosition()
    => FileWithPositionExists()
      ? ReadPositionFromFile()
      : Position.Start;

  private static bool FileWithPositionExists() => File.Exists(_file);

  private static Position ReadPositionFromFile()
    => Position.TryParse(File.ReadAllText(_file), out Position? position)
      ? position ?? Position.Start
      : Position.Start;

  private async Task HandleReceiptCreationAsync(ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
  {
    string jsonFact = System.Text.Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
    OpenNewReceiptFact receiptCreated = JsonSerializer.Deserialize<OpenNewReceiptFact>(jsonFact)!;
    CreateReceiptCommand command = new(receiptCreated.Id, receiptCreated.Store, receiptCreated.PurchaseDate);
    await _sender.SendAsync(command, cancellationToken);
  }
}
