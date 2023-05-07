using Microsoft.OpenApi.Models;
using SpoRE.Infrastructure.Database;
using SpoRE.Services;

namespace SpoRE.Setup;

internal static class Telemetry
{
    public static void AddServicesAndClients(this IServiceCollection services)
    {
        services.AddScoped<AccountService, AccountService>();
        services.AddScoped<AccountClient, AccountClient>();
        services.AddScoped<TeamSelectionService, TeamSelectionService>();
        services.AddScoped<TeamSelectionClient, TeamSelectionClient>();
        services.AddScoped<StageSelectionClient, StageSelectionClient>();
        services.AddScoped<StageClient, StageClient>();
        services.AddScoped<StageResultsClient, StageResultsClient>();
        services.AddScoped<RaceClient, RaceClient>();
        services.AddScoped<RaceService, RaceService>();
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

