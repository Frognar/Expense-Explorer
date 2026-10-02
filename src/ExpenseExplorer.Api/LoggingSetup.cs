using Serilog;
using Serilog.Events;

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

    /// <summary>
    /// Failed requests are errors. Files of the frontend (served without an endpoint) are only
    /// debug noise: a single page load fetches dozens of them and would bury the API calls.
    /// </summary>
    public static LogEventLevel RequestLevel(HttpContext http, double elapsedMilliseconds, Exception? exception) =>
        (exception, http.Response.StatusCode, http.GetEndpoint()) switch
        {
            (not null, _, _) or (_, >= StatusCodes.Status500InternalServerError, _) => LogEventLevel.Error,
            (_, < StatusCodes.Status400BadRequest, null) => LogEventLevel.Debug,
            _ => LogEventLevel.Information,
        };
}
