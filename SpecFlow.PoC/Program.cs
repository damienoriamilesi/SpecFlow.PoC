using System.Net.Mime;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using HealthChecks.Sqlite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using SpecFlow.PoC.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using SpecFlow.PoC;
using SpecFlow.PoC.Controllers;
using SpecFlow.PoC.Meters;

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
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// Add services to the container.
builder.Services.AddControllers(options => {
    options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
    options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
    options.Filters.Add<DataProtectionActionFilter>();
    //options.ReturnHttpNotAcceptable = true; // Required for Content Negotiation 
});
builder.Services.AddApiVersioning(options => {
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddMvc().AddApiExplorer(options => {
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddHealthChecks().AddCheck("SQLite Db", new SqliteHealthCheck("Data Source=SQLiteSample.db", $"SELECT 1 FROM {nameof(Employee)}s"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

//TODO > Implement
builder.Services.AddResponseCaching();

builder.Services.RegisterOpenTelemetry();

builder.Services.AddTransient<HttpClientMetricsMessageHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"{description.GroupName}/swagger.json", $"{description.GroupName.ToUpperInvariant()} - test");
        }
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