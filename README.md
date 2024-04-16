# Add Open API Standard Support


# Add Metrics with Prometheus / Grafana

Install dotnet counters
>dotnet tool install --global dotnet-counters 

Le répertoire d'outils '/Users/xxx/.dotnet/tools' n'est pas dans la variable d'environnement PATH.
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

export PATH="$PATH:/Users/xxx/.dotnet/tools"

Install Prometheus and Grafana as a Docker container
-



# HealthChecks

Ajouter le middleware de prise en charge des healthchecks


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
