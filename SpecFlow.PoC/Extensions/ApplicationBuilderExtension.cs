using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SpecFlow.PoC.Controllers;
using Swashbuckle.AspNetCore.SwaggerGen;

#pragma warning disable CS1591

namespace SpecFlow.PoC.Extensions;

public static class SwaggerBuilderExtension
{
    public static void AddOpenApiDocumentationSecurity(this IServiceCollection services) => services.AddSwaggerGen();
    public static OpenApiInfo GetOpenApiInfo(ApiVersionDescription apiVersionDescription)
    {
        return new OpenApiInfo
        {
            Title = "WeatherAPI",
            Description = "TODO > Describe",
            Version = apiVersionDescription.GroupName + "API",
            TermsOfService = new Uri("https://www.google.fr"),
            Contact = new OpenApiContact
            {
                Name = "Author",
                Url = new Uri("https://www.contact@gmail.com"),
                Email = "toto@gmail.com"
            },
            License = new OpenApiLicense
            {
                Name = "License",
                Url = new Uri("https://www.dao.fr")
            }
        };
    }
}

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly IConfiguration _configuration;
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration;
    }

    /// <summary>
    /// Configure each API discovered for Swagger Documentation
    /// </summary>
    /// <param name="options"></param>
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
        }
    }

    /// <summary>
    /// Configure Swagger Options. Inherited from the Interface
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
        
        // Include 'SecurityScheme' to use JWT Authentication
        var jwtSecurityScheme = new OpenApiSecurityScheme {
            BearerFormat = "JWT",
            Name = "JWT Authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Put **_ONLY_** your JWT Bearer token in the textbox below!",
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };
        var oidcSecurityScheme = new OpenApiSecurityScheme {
            Type = SecuritySchemeType.OpenIdConnect,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(_configuration["Secrets:AuthorizationUrl"]!),
                    Scopes = new Dictionary<string, string> { { "openid", "openid" }, { "profile", "profile" } }
                },
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(_configuration["Secrets:AuthorizationUrl"]),

                }
            }
        };

        //Add Bearer scheme
        options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
        //Add OIDC
        options.AddSecurityDefinition("Keycloak", oidcSecurityScheme);
        
        options.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtSecurityScheme, Array.Empty<string>() } });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement { { oidcSecurityScheme, Array.Empty<string>() } });
            
        options.IncludeXmlComments(typeof(WeatherForecastController).Assembly);
    }

    /// <summary>
    /// Create information about the version of the API
    /// </summary>
    /// <param name="apiDescription"></param>
    /// <returns>Information about the API</returns>
    private OpenApiInfo CreateVersionInfo(ApiVersionDescription apiDescription)
    {
        var info = SwaggerBuilderExtension.GetOpenApiInfo(apiDescription);
        if (apiDescription.IsDeprecated) info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
        return info;
    }
}