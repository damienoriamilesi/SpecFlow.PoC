using System.Reflection;
using HealthChecks.Sqlite;
using Microsoft.OpenApi.Models;
using Prometheus;
using SpecFlow.PoC.Features;
using Microsoft.EntityFrameworkCore;
using SpecFlow.PoC;


var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDataProtection();
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            options.UseSqlite("Data Source=SQLiteSample.db")
        );
        
        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddHealthChecks()
            .AddCheck("SQLite Db", new SqliteHealthCheck("SQLiteSample.db", nameof(Person)));  
            
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        builder.Services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WeatherAPI",
                    Description = "TODO > Describe",
                    Version = "v1",
                    TermsOfService = new Uri("https://www.google.fr"),
                    Contact = new OpenApiContact
                    {
                        Name = "Author",
                        Url = new Uri("https://www.contact@gmail.com"),
                        Email = "toto@gmail.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = new Uri("https://www.dao.fr")
                    }
                });
                
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

        builder.Services.AddTransient<HttpClientMetricsMessageHandler>();
        
        var app = builder.Build();
        
        // Ensure database is created during application startup
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        
        if (!dbContext.Employees.Any())
        {
            dbContext.Employees.AddRange(TestFixture.BuildEmployees());
            dbContext.SaveChanges();
        }

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

        //app.UseAuthorization();
        app.UseEndpoints(endpoint =>
        {
            endpoint.MapMetrics();
            endpoint.MapControllers();
            endpoint.MapHealthChecks("health");
        });

        app.Run();




