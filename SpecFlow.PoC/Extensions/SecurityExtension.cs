using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SpecFlow.PoC.Extensions;

/// <summary>
/// Register Security component
/// </summary>
public static  class SecurityExtension
{
    /// <summary>
    /// Add API Authentication (JWT and Bearer autnetication)
    /// </summary>
    /// <param name="services"></param>
    public static void AddApiAuthentication(this IServiceCollection services)
    {
        //http://localhost:8080/realms/DEV/.well-known/openid-configuration
        //http://localhost:8080/realms/DEV/protocol/openid-connect/certs
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Audience = "CoreBusinessService";
                options.IncludeErrorDetails = true;
                options.Authority = "http://localhost:8080/realms/DEV";
                var jwkFileContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jwk.json"));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // If authority is specified, no need to validate signature
                    ValidIssuer = "http://localhost:8080/realms/DEV",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new JsonWebKey(jwkFileContent),
                    //ValidAudience = "CoreBusinessService", 
                    //ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

    }
}