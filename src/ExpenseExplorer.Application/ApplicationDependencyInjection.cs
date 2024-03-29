namespace ExpenseExplorer.Application;

using System.Reflection;
using ExpenseExplorer.Application.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ApplicationDependencyInjection
{
  public static void AddApplication(this IServiceCollection services)
  {
    Assembly assembly = typeof(ApplicationDependencyInjection).Assembly;
    services.RegisterHandlers(assembly);
    services.AddScoped<ISender>(sp => CommandRegistry.CreateSender(sp.GetRequiredService, assembly));
  }

  private static void RegisterHandlers(this IServiceCollection services, Assembly assembly)
  {
    var descriptors = CommandRegistry.GetHandlerTypes(assembly)
      .Select(t => new ServiceDescriptor(t.Interface, t.Implementation, ServiceLifetime.Scoped));

    services.TryAdd(descriptors);
  }
}
