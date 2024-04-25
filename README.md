# Add Open API Standard Support


# Add Metrics with Prometheus / Grafana


<details>

<summary>See details</summary>

## <b>Install Prometheus and Grafana as a Docker container


https://docs.docker.com/config/daemon/prometheus/



### PROMETHEUS

> docker run --name my-prometheus --mount type=bind,source=/tmp/prometheus.> yml,destination=/etc/prometheus/prometheus.yml -p 9090:9090 prom/prometheus



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

# Vault Hashicorp?