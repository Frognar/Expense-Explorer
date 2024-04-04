namespace CommandHub.DependencyInjection;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class CommandHubDependencyInjection
{
  public static void AddCommandHub(this IServiceCollection services, params Assembly[] assemblies)
  {
    services.RegisterHandlers(assemblies);
    services.AddSingleton<ISender>(sp => CommandRegistry.CreateSender(GetRequiredService(sp), assemblies));
  }

  private static Func<Type, object> GetRequiredService(IServiceProvider sp)
  {
    IServiceScope scope = sp.GetRequiredService<IServiceScopeFactory>().CreateScope();
    return type => scope.ServiceProvider.GetRequiredService(type);
  }

  private static void RegisterHandlers(this IServiceCollection services, Assembly[] assemblies)
  {
    var descriptors = CommandRegistry.GetHandlerTypes(assemblies)
      .Select(t => new ServiceDescriptor(t.Interface, t.Implementation, ServiceLifetime.Transient));

    services.TryAdd(descriptors);
  }
}
