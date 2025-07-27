using Microsoft.OpenApi.Models;
using SpoRE.Services;
using SpoRE.Services.StageSelection;

namespace SpoRE.Setup;

internal static class Telemetry
{
    public static void AddServicesAndClients(this IServiceCollection services)
    {
        services.AddScoped<AccountService>();
        services.AddScoped<TeamSelectionService>();
        services.AddScoped<StageSelectionService>();
        services.AddScoped<RaceService>();
        services.AddScoped<RiderService>();
        services.AddScoped<StageResultService>();
        services.AddScoped<StatisticsService>();
        services.AddScoped<RaceWrapService>();
        services.AddSingleton<Scheduler>();
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

