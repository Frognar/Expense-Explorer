namespace ExpenseExplorer.Application;

using ExpenseExplorer.Application.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ApplicationDependencyInjection
{
  public static void AddApplication(this IServiceCollection services)
  {
    services.RegisterCommandHandlers();
    Dictionary<Type, BaseCommandHandlerWrapper> wrappers = CreateCommandHandlerWrappers();
    services.AddScoped<ISender>(sp => new Sender(sp, wrappers));
  }

  private static void RegisterCommandHandlers(this IServiceCollection services)
  {
    IEnumerable<ServiceDescriptor> serviceDescriptors = GetClassesImplementingInterface(typeof(ICommandHandler<,>))
      .Select(type => new { type, inter = type.GetInterface("ICommandHandler`2")! })
      .Select(x => new ServiceDescriptor(x.inter, x.type, ServiceLifetime.Scoped));

    services.TryAdd(serviceDescriptors);
  }

  private static IEnumerable<Type> GetClassesImplementingInterface(Type interfaceType)
  {
    return typeof(ApplicationDependencyInjection).Assembly.ExportedTypes
      .Where(
        type =>
        {
          IEnumerable<Type> genericInterfaceTypes = type.GetInterfaces().Where(i => i.IsGenericType);
          bool implementRequestTypes = genericInterfaceTypes
            .Any(i => i.GetGenericTypeDefinition() == interfaceType);

          return type is { IsInterface: false, IsAbstract: false } && implementRequestTypes;
        });
  }

  private static Dictionary<Type, BaseCommandHandlerWrapper> CreateCommandHandlerWrappers()
  {
    return GetClassesImplementingInterface(typeof(ICommand<>))
      .ToDictionary(command => command, CreateWrapper);
  }

  private static BaseCommandHandlerWrapper CreateWrapper(Type commandType)
  {
    Type wrapperType = typeof(CommandHandlerWrapperImpl<,>).MakeGenericType(
      commandType,
      commandType.GetInterface("ICommand`1")!.GetGenericArguments()[0]);

    object wrapper = Activator.CreateInstance(wrapperType)
                     ?? throw new InvalidOperationException($"Failed to create instance of {wrapperType}.");

    return (BaseCommandHandlerWrapper)wrapper;
  }
}
