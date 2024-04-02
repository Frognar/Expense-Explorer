namespace CommandHub;

using System.Reflection;
using CommandHub.Commands;
using CommandHub.Queries;
using CommandHub.Wrappers;

public static class CommandRegistry
{
  public static ISender CreateSender(Func<Type, object> serviceProvider, params Assembly[] assemblies)
  {
    return new Sender(serviceProvider, CreateWrappers(assemblies));
  }

  public static IEnumerable<(Type Interface, Type Implementation)> GetHandlerTypes(params Assembly[] assemblies)
  {
    IEnumerable<(Type, Type)> commandHandlerWrappers = GetClassesImplementing(typeof(ICommandHandler<,>), assemblies)
      .Select(type => (type.GetInterface(typeof(ICommandHandler<,>).Name)!, type));

    IEnumerable<(Type, Type)> queryHandlerWrappers = GetClassesImplementing(typeof(IQueryHandler<,>), assemblies)
      .Select(type => (type.GetInterface(typeof(IQueryHandler<,>).Name)!, type));

    return commandHandlerWrappers.Concat(queryHandlerWrappers);
  }

  private static Dictionary<Type, BaseHandlerWrapper> CreateWrappers(Assembly[] assemblies)
  {
    Dictionary<Type, BaseHandlerWrapper> commandHandlerWrappers = GetClassesImplementing(typeof(ICommand<>), assemblies)
      .ToDictionary(commandType => commandType, CreateCommandHandlerWrapper);

    Dictionary<Type, BaseHandlerWrapper> queryHandlerWrappers = GetClassesImplementing(typeof(IQuery<>), assemblies)
      .ToDictionary(queryType => queryType, CreateQueryHandlerWrapper);

    return commandHandlerWrappers
      .Concat(queryHandlerWrappers)
      .ToDictionary(kv => kv.Key, kv => kv.Value);
  }

  private static BaseHandlerWrapper CreateCommandHandlerWrapper(Type commandType)
  {
    Type wrapperType = typeof(CommandHandlerWrapperImpl<,>).MakeGenericType(
      commandType,
      commandType.GetInterface(typeof(ICommand<>).Name)!.GetGenericArguments()[0]);

    object wrapper = Activator.CreateInstance(wrapperType)
                     ?? throw new InvalidOperationException($"Failed to create instance of {wrapperType}.");

    return (BaseHandlerWrapper)wrapper;
  }

  private static BaseHandlerWrapper CreateQueryHandlerWrapper(Type queryType)
  {
    Type wrapperType = typeof(QueryHandlerWrapperImpl<,>).MakeGenericType(
      queryType,
      queryType.GetInterface(typeof(IQuery<>).Name)!.GetGenericArguments()[0]);

    object wrapper = Activator.CreateInstance(wrapperType)
                     ?? throw new InvalidOperationException($"Failed to create instance of {wrapperType}.");

    return (BaseHandlerWrapper)wrapper;
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
