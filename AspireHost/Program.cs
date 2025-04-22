using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var cache = builder.AddRedis("cache").WithRedisCommander();

var postgres = builder.AddPostgres("postgres").PublishAsConnectionString();
var db = postgres.AddDatabase("Db");// Internal API

// Add docker run -p 9090:9090 prom/prometheus

// Add Keycloak
//var keycloak = builder.AddKeycloak("keycloak", 8080);

//...

builder.AddProject<SpecFlow_PoC>("api")
        .WithReference(cache);

builder.Build().Run();

