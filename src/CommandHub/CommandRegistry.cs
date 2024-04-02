namespace CommandHub;

using System.Reflection;
using CommandHub.Commands;
using CommandHub.Commands.Wrappers;

public static class CommandRegistry
{
  public static ISender CreateSender(Func<Type, object> serviceProvider, params Assembly[] assemblies)
  {
    return new Sender(serviceProvider, CreateWrappers(assemblies));
  }

  public static IEnumerable<(Type Interface, Type Implementation)> GetHandlerTypes(params Assembly[] assemblies)
  {
    return GetClassesImplementing(typeof(ICommandHandler<,>), assemblies)
      .Select(type => (type.GetInterface(typeof(ICommandHandler<,>).Name)!, type));
  }

  private static Dictionary<Type, BaseCommandHandlerWrapper> CreateWrappers(IEnumerable<Assembly> assemblies)
  {
    return GetClassesImplementing(typeof(ICommand<>), assemblies)
      .ToDictionary(command => command, CreateWrapper);
  }

  private static BaseCommandHandlerWrapper CreateWrapper(Type commandType)
  {
    Type wrapperType = typeof(CommandHandlerWrapperImpl<,>).MakeGenericType(
      commandType,
      commandType.GetInterface(typeof(ICommand<>).Name)!.GetGenericArguments()[0]);

    object wrapper = Activator.CreateInstance(wrapperType)
                     ?? throw new InvalidOperationException($"Failed to create instance of {wrapperType}.");

    return (BaseCommandHandlerWrapper)wrapper;
  }

  private static IEnumerable<Type> GetClassesImplementing(Type interfaceType, IEnumerable<Assembly> assemblies)
  {
    return assemblies.SelectMany(GetClassesImplementing(interfaceType));
  }

  private static Func<Assembly, IEnumerable<Type>> GetClassesImplementing(Type interfaceType)
  {
    return assembly => GetClassesImplementing(interfaceType, assembly);
  }

  private static IEnumerable<Type> GetClassesImplementing(Type interfaceType, Assembly assembly)
  {
    return assembly.ExportedTypes.Where(IsImplementing(interfaceType));
  }

  private static Func<Type, bool> IsImplementing(Type interfaceType)
  {
    return classType => IsImplementing(interfaceType, classType);
  }

  private static bool IsImplementing(Type interfaceType, Type type)
  {
    IEnumerable<Type> genericInterfaceTypes = type.GetInterfaces().Where(i => i.IsGenericType);
    bool implementRequestTypes = genericInterfaceTypes.Any(i => i.GetGenericTypeDefinition() == interfaceType);
    return type is { IsInterface: false, IsAbstract: false } && implementRequestTypes;
  }
}
