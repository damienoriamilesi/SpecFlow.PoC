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
        options.RequireHttpsMetadata = false;
        options.Authority = "http://localhost:8080/realms/DEV";
        options.Audience = "CoreBusinessService";
        //options.SaveToken = true;
        //options.
        
        var jwkFileContent = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jwk.json"));
        var jsonWebKeySet = new JsonWebKeySet(jwkFileContent);
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            //IssuerSigningKey = 
                //new JsonWebKey(jwkFileContent),
            //IssuerSigningKey = 
                //new SymmetricSecurityKey(Convert.FromBase64String("KyOr6CRQmsbyxCAki2ZJmMz7HpiwpZkJ")), 
                ValidAudience = "CoreBusinessService", 
            ClockSkew = TimeSpan.Zero,
            //IssuerSigningKeyValidator = (key, token, parameters) => 
            //IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) => jsonWebKeySet.Keys.Select(x=>x.),

            //IssuerSigningKeyResolverUsingConfiguration = 
            //SignatureValidator = (token, _) => new JsonWebToken(token)
            /*SignatureValidator = (token, x) =>
            {
                 var result = new JsonWebTokenHandler().ValidateToken(token, x);
                 if (result.IsValid) return new JsonWebToken(token);
                 throw new SecurityTokenException("bvzbveizmb");
            }*/
        };
    });

builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});

// Add services to the container.
builder.Services.AddControllers(config =>
{
    config.Filters.Add<DatProtectionActionFilter>();
    //config.ReturnHttpNotAcceptable = true; // Required for Content Negotiation 
});

builder.Services.AddHealthChecks()
    .AddCheck("SQLite Db", 
        new SqliteHealthCheck("Data Source=SQLiteSample.db", $"SELECT 1 FROM {nameof(Employee)}s"));
        
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
builder.Services.AddOpenApiDocumentation(builder.Configuration);

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
        options.SwaggerEndpoint("v1/swagger.json", "WeatherAPI - v1"); 
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