using Microsoft.OpenApi.Models;
using SpoRE.Infrastructure.Base;
using SpoRE.Infrastructure.Database.Account;
using SpoRE.Infrastructure.Database.Teamselection;
using SpoRE.Infrastructure.Database.Stage;
using SpoRE.Services;

namespace SpoRE.Setup;

internal static class Telemetry
{
    public static void AddServicesAndClients(this IServiceCollection services)
    {
        services.AddScoped<SqlDatabaseAdapter, SqlDatabaseAdapter>();

        services.AddScoped<AccountService, AccountService>();
        services.AddScoped<AccountClient, AccountClient>();

        services.AddScoped<TeamselectionService, TeamselectionService>();
        services.AddScoped<TeamselectionClient, TeamselectionClient>();

        services.AddScoped<StageService, StageService>();
        services.AddScoped<StageClient, StageClient>();
    }

    public static void AddSwaggerLogin(this IServiceCollection services)
    {
        services.AddSwaggerGen(
            c =>
            {
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "Paste a JWT token here"
                    });
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
            });
    }
}

