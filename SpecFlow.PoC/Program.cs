﻿using System.Reflection;
using Microsoft.OpenApi.Models;
using Prometheus;

namespace SpecFlow.PoC;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers().AddNewtonsoftJson();
          
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        builder.Services.AddSwaggerGen(x=> x.SwaggerDoc("v1", new OpenApiInfo {
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
        }));
        
        var app = builder.Build();

        app.UseMetricServer(options =>
        {
            //options.Registry.CollectAndExportAsTextAsync()
        });//Starting the metrics exporter, will expose "/metrics"

        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseHsts();
            app.UseSwagger();
            //app.UseSwaggerUI();
            app.UseSwaggerUI(options =>
            {
                //options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                //options.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();

        //adding metrics related to HTTP
        app.UseHttpMetrics(options=>
        {
            options.AddCustomLabel("host", context => context.Request.Host.Host);
        });

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

