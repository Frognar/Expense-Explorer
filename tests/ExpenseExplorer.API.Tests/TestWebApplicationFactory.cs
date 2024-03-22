namespace ExpenseExplorer.API.Tests;

using ExpenseExplorer.Application.Receipts.Persistence;
using ExpenseExplorer.Infrastructure.Receipts.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

internal sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(sc => Replace<TimeProvider, TestTimeProvider>(sc, new TestTimeProvider()));
    builder.ConfigureServices(
      sc => Replace<IReceiptRepository, InMemoryReceiptRepository>(sc, new InMemoryReceiptRepository()));
  }

  private static void Replace<T, U>(IServiceCollection services, U replacement)
    where U : class, T
    where T : class
  {
    ServiceDescriptor? descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
    if (descriptor is not null)
    {
      services.Remove(descriptor);
    }

    services.AddScoped<T, U>(_ => replacement);
  }

  private sealed class TestTimeProvider : TimeProvider
  {
    public override DateTimeOffset GetUtcNow()
    {
      return new DateTimeOffset(Today, TimeSpan.Zero);
    }
  }
}
