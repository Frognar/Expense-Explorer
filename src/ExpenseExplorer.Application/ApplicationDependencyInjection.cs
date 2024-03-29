namespace ExpenseExplorer.Application;

using CommandHub.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

public static class ApplicationDependencyInjection
{
  public static void AddApplication(this IServiceCollection services)
  {
    services.AddCommandHub(typeof(ApplicationDependencyInjection).Assembly);
  }
}
