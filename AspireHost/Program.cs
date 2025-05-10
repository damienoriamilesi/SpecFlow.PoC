using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithRedisCommander();

var postgres =
    builder.AddPostgres("my-postgres-db")
        .WithPgAdmin()
        .WithDataVolume()
        .PublishAsConnectionString();
var db = postgres.AddDatabase("weather-db");// Internal API

// Add docker run -p 9090:9090 prom/prometheus

// Add Keycloak
//var keycloak = builder.AddKeycloak("keycloak", 8080);

//...

builder.AddProject<SpecFlow_PoC>("weatherforecast-api")
    .WithReference(db)
    .WithReference(cache);

builder.Build().Run();

