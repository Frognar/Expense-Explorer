namespace CommandHub.DependencyInjection;

using System.Reflection;
using CommandHub.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class CommandHubDependencyInjection
{
  public static void AddCommandHub(this IServiceCollection services, params Assembly[] assemblies)
  {
    services.RegisterHandlers(assemblies);
    services.AddScoped<ISender>(sp => CommandRegistry.CreateSender(sp.GetRequiredService, assemblies));
  }

  private static void RegisterHandlers(this IServiceCollection services, Assembly[] assemblies)
  {
    var descriptors = CommandRegistry.GetHandlerTypes(assemblies)
      .Select(t => new ServiceDescriptor(t.Interface, t.Implementation, ServiceLifetime.Scoped));

    services.TryAdd(descriptors);
  }
}
