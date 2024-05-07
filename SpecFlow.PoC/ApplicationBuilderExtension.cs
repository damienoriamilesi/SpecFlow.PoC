using System.Reflection;
using Microsoft.OpenApi.Models;
using SpecFlow.PoC;

public static class ApplicationBuilderExtension
{
    public static void AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo{ Title = "My sample API", Version = "v1" });
            options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://your-keycloak-server/realms/your-realm/protocol/openid-connect/auth"),
                        Scopes = new Dictionary<string, string> { { "openid", "openid" }, { "profile", "profile" } }
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
}