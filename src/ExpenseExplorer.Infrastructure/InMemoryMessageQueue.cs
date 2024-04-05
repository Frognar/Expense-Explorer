namespace ExpenseExplorer.Infrastructure;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using ExpenseExplorer.Domain.Receipts.Facts;

[SuppressMessage(
  "Performance",
  "CA1812:Avoid uninstantiated internal classes",
  Justification = "Instantiated by DI container")]
internal sealed class InMemoryMessageQueue
{
  private readonly Channel<Fact> _channel = Channel.CreateUnbounded<Fact>();

  public ChannelReader<Fact> Reader => _channel.Reader;

  public ChannelWriter<Fact> Writer => _channel.Writer;
}
