using ExpenseExplorer.Application.Dictionaries;
using ExpenseExplorer.Application.ReceiptItems;
using ExpenseExplorer.Application.Receipts;
using ExpenseExplorer.Application.Reports;
using ExpenseExplorer.Infrastructure.Dictionaries;
using ExpenseExplorer.Infrastructure.Persistence;
using ExpenseExplorer.Infrastructure.ReceiptItems;
using ExpenseExplorer.Infrastructure.Receipts;
using ExpenseExplorer.Infrastructure.Reports;
using ExpenseExplorer.Application.Users;
using ExpenseExplorer.Domain.Users;
using ExpenseExplorer.Infrastructure.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ExpenseExplorer.Infrastructure;

public static class InfrastructureSetup
{
    public const string ConnectionStringName = "expense-explorer";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // The database is reached with a password on a private network; without this Npgsql
        // probes for Kerberos and the container, which has no Kerberos library, prints a warning.
        NpgsqlConnectionStringBuilder connection = new(connectionString) { GssEncryptionMode = GssEncryptionMode.Disable };
        services.AddDbContext<ExpenseExplorerDbContext>(options => options.UseNpgsql(
            connection.ConnectionString,
            npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", ExpenseExplorerDbContext.Schema)));
        services.AddScoped<IReceiptRepository, ReceiptRepository>();
        services.AddScoped<IReceiptQueries, ReceiptQueries>();
        services.AddScoped<IDictionaryQueries, DictionaryQueries>();
        services.AddScoped<IDictionaryEditor, DictionaryEditor>();
        services.AddScoped<INameHistory, NameHistory>();
        services.AddScoped<IReceiptItemQueries, ReceiptItemQueries>();
        services.AddScoped<IReportQueries, ReportQueries>();
        services.AddUsers();
        return services;
    }

    /// <summary>Brings the database schema up to date. On the first run it also copies the data of the previous app version.</summary>
    public static async Task MigrateDatabaseAsync(this IServiceProvider services)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        ExpenseExplorerDbContext db = scope.ServiceProvider.GetRequiredService<ExpenseExplorerDbContext>();
        await db.Database.MigrateAsync();
    }

    private static void AddUsers(this IServiceCollection services)
    {
        services.AddIdentityCore<UserAccount>(options =>
            {
                options.Password.RequiredLength = Password.MinLength;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.-_";
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ExpenseExplorerDbContext>();
        services.AddScoped<IUserAccounts, IdentityUserAccounts>();
        services.AddScoped<IRefreshTokens, RefreshTokenStore>();
    }
}
