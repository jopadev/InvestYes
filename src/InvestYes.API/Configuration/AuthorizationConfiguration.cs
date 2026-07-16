using Microsoft.AspNetCore.Authorization;

namespace InvestYes.API.Configuration;

public static class AuthorizationConfiguration
{
    public static IServiceCollection AddJwtAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Administrator",
                policy => policy.RequireRole("Administrator"));

            options.AddPolicy("Investor",
                policy => policy.RequireRole("Investor"));
        });

        return services;
    }
}