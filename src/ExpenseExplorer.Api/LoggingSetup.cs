using Serilog;

namespace ExpenseExplorer.Api;

internal static class LoggingSetup
{
    /// <summary>Sinks and levels come from the "Serilog" section of appsettings.</summary>
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, logger) => logger
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName));

        return builder;
    }
}
