using Microsoft.EntityFrameworkCore;
using Prometheus;
using SpecFlow.PoC;
using SpecFlow.PoC.Extensions;
using SpecFlow.PoC.Extensions.OpenApiSpec;
using SpecFlow.PoC.Features;
using SpecFlow.PoC.Meters;

var builder = WebApplication.CreateBuilder(args);

// Aspire services
builder.AddServiceDefaults();

#region Add services to the IoC Container

// Add Problem Details support
builder.Services.AddProblemDetails();
builder.Services.ConfigureOptions<ConfigureProblemDetailsOptions>();

builder.Services.AddControllers();
builder.Services.ConfigureOptions<ConfigureControllersOptions>();

builder.Services.AddApiVersioning().AddMvc().AddApiExplorer(options => { options.GroupNameFormat = "'v'V"; options.SubstituteApiVersionInUrl = true; });
builder.Services.ConfigureOptions<ConfigureVersioningOptions>();

builder.Services.AddApiAuthentication();

builder.Services.AddDatabase();

builder.Services.AddHealthchecks();
builder.Services.AddDataProtection();


builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

//TODO > Implement
//builder.Services.AddRedis();
builder.Services.AddResponseCaching();
builder.Services.RegisterOpenTelemetry();
builder.Services.AddTransient<HttpClientMetricsMessageHandler>();
builder.Services.AddHttpContextAccessor();

#endregion

#region Configure HTTP Request Pipeline

//////////////////////////////////////////////////////////////////////////////////////////
//      MIDDLEWARES ORDER
//////////////////////////////////////////////////////////////////////////////////////////
/*
 * ExceptionHandler
 * HSTS
 * HttpsRedirection
 * StaticFiles
 * Routing
 * CORS
 * Authentication
 * Authorization
 *
 *
 * 
 * CUSTOM ONES
 *
 *
 * ENDPOINTS
 */
//////////////////////////////////////////////////////////////////////////////////////////
var app = builder.Build();
// Ensure database is created during application startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    
    //dbContext.Database.Migrate();

    if (!dbContext.Employees.Any())
    {
        dbContext.Employees.AddRange(TestFixture.BuildEmployees());
        dbContext.SaveChanges();
    }
    
    if (!dbContext.WeatherForecasts.Any())
    {
        dbContext.WeatherForecasts.Add(new WeatherForecast { Date = DateTime.Now, TemperatureC = 10, Summary = "Freezing" });
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseHsts();
    app.UseOpenApiSpecVersioning();
//}
app.UseHttpsRedirection();
app.UseRouting();

//adding metrics related to HTTP
/*
app.UseMetricServer(options =>
{
//options.Registry.CollectAndExportAsTextAsync()
});//Starting the metrics exporter, will expose "/metrics"
*/

app.UseHttpMetrics(options=> {
    options.AddCustomLabel("host", context => context.Request.Host.Host);
    //options.AddCustomLabel("http", context => context.Request.HttpContext.);
});

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();

app.MapMetrics();
app.MapControllers().RequireAuthorization();


app.MapDefaultEndpoints();

#endregion

app.Run();