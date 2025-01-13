using System.Net.Mime;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using SpecFlow.PoC;
using SpecFlow.PoC.Controllers;
using SpecFlow.PoC.Extensions;
using SpecFlow.PoC.Meters;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddDataProtection();

builder.Services.AddDatabase();

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
}).AddMvc()
    .AddApiExplorer(options => {
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddHealthchecks();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

//TODO > Implement
//builder.Services.AddRedis()
builder.Services.AddResponseCaching();

builder.Services.RegisterOpenTelemetry();

builder.Services.AddTransient<HttpClientMetricsMessageHandler>();

builder.Services.AddHttpContextAccessor();

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

app.UseHttpMetrics(options=> {
    options.AddCustomLabel("host", context => context.Request.Host.Host);
    //options.AddCustomLabel("http", context => context.Request.HttpContext.);
});

app.UseAuthentication();
app.UseAuthorization();

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