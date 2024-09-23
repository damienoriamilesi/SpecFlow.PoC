using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using SpecFlow.PoC;
using SpecFlow.PoC.Controllers;

#pragma warning disable CS1591

public static class ApplicationBuilderExtension
{
    public static void AddOpenApiDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "My sample API", Version = "v1" });
            options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(configuration["Secrets:AuthorizationUrl"]),
                        Scopes = new Dictionary<string, string> { { "openid", "openid" }, { "profile", "profile" } }
                    }
                    , AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(configuration["Secrets:AuthorizationUrl"]),
                        
                    }
                }
            });
    
            OpenApiSecurityScheme keycloakSecurityScheme = new()
            {
                Reference = new OpenApiReference
                {
                    Id = "Keycloak",
                    Type = ReferenceType.SecurityScheme,
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer",
            };

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { keycloakSecurityScheme, Array.Empty<string>() },
            });
    
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
    }

    public static void AddOpenApiDocumentationSecurity(this IServiceCollection services)
    {
        services.AddSwaggerGen(setup =>
        {
            // Include 'SecurityScheme' to use JWT Authentication
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
            
            setup.IncludeXmlComments(typeof(WeatherForecastController).Assembly);
        });
    }
    public static void AddOpenApiStandardBasic(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WeatherAPI",
                Description = "TODO > Describe",
                Version = "v1",
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
            });
                
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

    }

    public static void AddEntityFramework(this IServiceProvider serviceProvider)
    {
        // Ensure database is created during application startup
        using var scope = serviceProvider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
        if (dbContext.Employees.Any()) return;
        dbContext.Employees.AddRange(TestFixture.BuildEmployees());
        dbContext.SaveChanges();
    }
    
    public static Task ValidateToken(MessageReceivedContext context)
    {
        try
        {
            //context.Token = GetToken(context.Request);

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(context.Token, context.Options.TokenValidationParameters, out var validatedToken);

            var jwtSecurityToken = validatedToken as JwtSecurityToken;

            context.Principal = new ClaimsPrincipal();

            var claimsIdentity = new ClaimsIdentity(jwtSecurityToken.Claims.ToList(),
                "JwtBearerToken", ClaimTypes.NameIdentifier, ClaimTypes.Role);
            context.Principal.AddIdentity(claimsIdentity);

            context.Success();

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            context.Fail(e);
        }

        return Task.CompletedTask;
    }
}