using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var cache = 
    builder.AddRedis("cache").WithRedisCommander();
builder.AddProject<SpecFlow_PoC>("api")
    .WithReference(cache);

builder.Build().Run();

//var postgres = builder.AddPostgres("postgres").PublishAsConnectionString();
//var db = postgres.AddDatabase("Db");// Internal API
//.WithReference(db);
