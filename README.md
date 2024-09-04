# Add Open API Standard Support

# Caching

Adding Response caching 
Just add middleware 

    builder.Services.AddResponseCaching(cfg => { });

And just BEFORE MapControllers

    app.UseResponseCaching();

https://code-maze.com/aspnetcore-response-caching/


# Add EF Core support (for SQLite)

Add the packages

    Microsoft.EntityFrameworkCore 
    Microsoft.EntityFrameworkCore.SQLite

Then Add the instructions to ensure database is created during application startup
        
    using var scope = app.Services.CreateScope();
    var dbContext = 
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

Finally, add the following to seed the Db if needed

    if (!dbContext.Employees.Any())
    {
        dbContext.Employees.AddRange(TestFixture.BuildEmployees());
        dbContext.SaveChanges();
    }

# Add Metrics with Prometheus / Grafana


<details>

<summary>See details</summary>

## <b>Install Prometheus and Grafana as a Docker container


https://docs.docker.com/config/daemon/prometheus/



### PROMETHEUS

Ajouter le fichier prometheus.yml de configuration dans un dossier identifié comme volume

> docker run --name my-prometheus --mount type=bind,source=/tmp/prometheus,destination=/etc/prometheus/prometheus -p 9090:9090 prom/prometheus



### GRAFANA

> docker run -d -p 3000:3000 --name=grafana grafana/grafana-enterprise

Open Grafana => Home > Connections > Add Data Source > Prometheus
Set the following value into Prometheus server URL: 
http://host.docker.internal:9090  


## <b>Install dotnet counters
<br>

> dotnet tool install --global dotnet-counters 

Le répertoire d'outils <i>'/Users/xxx/.dotnet/tools'</i> n'est pas dans la variable d'environnement PATH.
Si vous utilisez zsh, vous pouvez l'ajouter à votre profil en exécutant la commande suivante :

> cat << \EOF >> ~/.zprofile
> 
 Ajouter les outils du kit SDK .NET Core
> export PATH="$PATH:/Users/xxx/.dotnet/tools"
> EOF

Exécutez ensuite 
> zsh -l

afin de le rendre disponible pour la session active.

Vous pouvez uniquement l'ajouter à la session active en exécutant la commande suivante :

> export PATH="$PATH:/Users/xxx/.dotnet/tools"

### Customize prometheus.yml



</details>

<br><br>

# HealthChecks

Add healthchecks middleware (Microsoft.Extensions.Diagnostics.HealthChecks)
just after AddControllers instruction.

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddHealthChecks();  

Add the following to check if Db is up and running
>             .AddCheck("SQLite Db", new SqliteHealthCheck("SQLiteSample.db", nameof(Person)));  


Customizing the response
> https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0#customize-output

<br>
<br>

# BDD - SpecFlow
## Hooks

Actions to perform before and/or after each feature, Scenario or Step
> https://docs.specflow.org/projects/specflow/en/latest/Bindings/Hooks.html

<br>

## Dependency Injection

TODO

<br>

## Generate Living Doc

./livingdoc test-assembly xxx/SpecFlow.PoC.BDD.Tests.dll -t xxx/TestExecution.json -o xxx/LivingDoc/PoC.BDD.Report.html

# KeyCloak

    docker run -p 8080:8080 -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin quay.io/keycloak/keycloak start-dev

Add the following instructions

    services.AddKeycloakAuthentication(builder.Configuration);
    services.AddAuthorization();

https://medium.com/@ahmed.gaduo_93938/how-to-implement-keycloak-authentication-in-a-net-core-application-ce8603698f24

https://nikiforovall.github.io/aspnetcore/dotnet/2022/08/24/dotnet-keycloak-auth.html

And this chunk

    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo{ Title = "My API", Version = "v1" });
        options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri("https://your-keycloak-server/realms/your-realm/protocol/openid-connect/auth"),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "openid" },
                        { "profile", "profile" }
                    }
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

# Vault Hashicorp?