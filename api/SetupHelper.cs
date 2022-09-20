using SpoRE.Infrastructure.Base;
using SpoRE.Infrastructure.Database.Account;
using SpoRE.Services;

namespace SpoRE.Setup;

internal static class Telemetry
{
    public static void AddServicesAndClients(this IServiceCollection services)
    {
        services.AddScoped<SqlDatabaseClient, SqlDatabaseClient>();

        services.AddScoped<AccountClient, AccountClient>();
        services.AddScoped<AccountService, AccountService>();
    }
}

