using FluentAssertions.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseExplorer.API.Tests;

internal class TestWebApplicationFactory : WebApplicationFactory<Program> {
  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    builder.ConfigureServices(Replace<TimeProvider, TestTimeProvider>);
  }

  private static void Replace<T, U>(IServiceCollection services) where U : class, T where T : class {
    ServiceDescriptor? descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
    if (descriptor is not null) {
      services.Remove(descriptor);
    }

    services.AddScoped<T, U>();
  }

  private class TestTimeProvider : TimeProvider {
    public override DateTimeOffset GetUtcNow() {
      return today.ToDateTimeOffset();
    }
  }
}
