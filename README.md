![example workflow](https://github.com/damienoriamilesi/SpecFlow.PoC/actions/workflows/dotnet.yml/badge.svg)


# Common topics

- Central package management

  https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management




# Open API Standard Support

<details>
<summary>See details</summary>
<br>
<p>Microcks to register the API contracts</p>
<br>

    docker run -p 8585:8080 -it --rm quay.io/microcks/microcks-uber:latest-native 

Microcks - Cron Job interval :  https://microcks.io/documentation/guides/usage/importing-content/
services.update.interval=${SERVICES_UPDATE_INTERVAL:0 0 0/2 * * *}

https://crontab.guru/#*_*_*_*_* (Help to format interval)

Example: https://raw.githubusercontent.com/damienoriamilesi/SpecFlow.PoC/master/SpecFlow.PoC/swagger.json

## Exec commands inside the container
    docker exec -it d619f95ec002 /bin/bash

</details>
<br>

# Caching

<details>
<summary>See details</summary>

See OutputCaching / HybridCaching in NET9
https://www.milanjovanovic.tech/blog/hybrid-cache-in-aspnetcore-new-caching-library


</details>
<br>

# EF Core support (for SQLite)

<details>
<summary>See details</summary>

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

</details>
<br>

#  Metrics with Prometheus / Grafana


<details>
<summary>See details</summary>

## <b>Install Prometheus and Grafana as a Docker container

> docker run -p 9090:9090 prom/prometheus

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
<br>

# HealthChecks

<details>
<summary>See details</summary>

Add healthchecks middleware (Microsoft.Extensions.Diagnostics.HealthChecks)
just after AddControllers instruction.

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddHealthChecks();  

Add the following to check if Db is up and running
>             .AddCheck("SQLite Db", new SqliteHealthCheck("SQLiteSample.db", nameof(Person)));  


Customizing the response
> https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0#customize-output

</details>
<br>

# BDD - SpecFlow

<details>
<summary>See details</summary>


## Hooks

Actions to perform before and/or after each feature, Scenario or Step
> https://docs.specflow.org/projects/specflow/en/latest/Bindings/Hooks.html

<br>

## Dependency Injection

TODO

<br>

## Generate Living Doc

./livingdoc test-assembly xxx/SpecFlow.PoC.BDD.Tests.dll -t xxx/TestExecution.json -o xxx/LivingDoc/PoC.BDD.Report.html

</details>
<br>

# KeyCloak

<details>
<summary>See details</summary>

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

Checking token / policies

https://rmauro.dev/jwt-authentication-with-csharp-dotnet/#set-up-authentication-and-authorization

</details>
<br>

# Vault Hashicorp?

# Aspire install

    sudo dotnet workload install aspire

First, we gotta add Project to the builder.
Minimal instruction to build:
    using Projects;

    var builder = DistributedApplication.CreateBuilder(args);
    builder.AddProject<SpecFlow_PoC>("api");

    builder.Build().Run();

NB: 
Possible Error => 
https://github.com/dotnet/aspire/issues/6851

    fail: Aspire.Hosting.Dcp.dcpctrl.NetworkReconciler[0]
    could not create the network as all available subnet ranges
    from the default pool are allocated, retrying...
    {
        "NetworkName": {"name":"default-aspire-network"}, 
        "Reconciliation": 16, 
        "Network": "default-aspire-network-o428ocs3sd", 
        "error": "object could not allocate required resources/nnetwork subnet pool full"
    }
SOLUTION => docker network prune