using HealthChecks.Sqlite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;
using SpecFlow.PoC.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpecFlow.PoC;
using SpecFlow.PoC.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();

builder.Services.AddEntityFrameworkSqlite().AddDbContext<ApplicationDbContext>(options =>
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    options.UseSqlite("Filename=SQLiteSample.db")
);

//http://localhost:8080/realms/DEV/.well-known/openid-configuration
//http://localhost:8080/realms/DEV/protocol/openid-connect/certs
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.Audience = "CoreBusinessService";
        
        options.IncludeErrorDetails = true;

        //options.Authority = "http://localhost:8080/realms/DEV";
        var jwkFileContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jwk.json"));
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // If authority is specified, no need to validate signature
            ValidIssuer = "http://localhost:8080/realms/DEV",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new JsonWebKey(jwkFileContent),
            //ValidAudience = "CoreBusinessService", 
            //ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddOpenApiDocumentationSecurity();

// Add services to the container.
builder.Services.AddControllers(config =>
{
    config.Filters.Add<DataProtectionActionFilter>();
    //config.ReturnHttpNotAcceptable = true; // Required for Content Negotiation 
});

builder.Services.AddHealthChecks()
    .AddCheck("SQLite Db", 
        new SqliteHealthCheck("Data Source=SQLiteSample.db", $"SELECT 1 FROM {nameof(Employee)}s"));
        
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddResponseCaching(cfg => { });

//DependencyInjection
builder.Services.AddTransient<HttpClientMetricsMessageHandler>();

var app = builder.Build();

//app.Services.AddEntityFramework();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseHsts();
    app.UseSwagger().UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("v1/swagger.json", "v1"); 
    });
}
//app.UseHttpsRedirection();
app.UseRouting();

//adding metrics related to HTTP
/*
app.UseMetricServer(options =>
{
//options.Registry.CollectAndExportAsTextAsync()
});//Starting the metrics exporter, will expose "/metrics"
*/

app.UseHttpMetrics(options=>
{
    options.AddCustomLabel("host", context => context.Request.Host.Host);
    //options.AddCustomLabel("http", context => context.Request.HttpContext.);
});

app.UseAuthentication();
app.UseAuthorization();
/*
app.Use(_ => { var tokenHandler = new JwtSecurityTokenHandler();
    try
    {
        tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        return validatedToken != null;
    }
    catch (Exception)
    {
        return false;
    } });
    
*/

app.UseResponseCaching();

app.UseEndpoints(endpoint =>
{
    endpoint.MapMetrics();
    endpoint.MapControllers().RequireAuthorization();
    endpoint.MapHealthChecks("health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckExtension.WriteResponse
    });
});

app.Run();