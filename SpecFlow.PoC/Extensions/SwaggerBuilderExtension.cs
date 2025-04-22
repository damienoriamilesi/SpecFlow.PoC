using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

#pragma warning disable CS1591

namespace SpecFlow.PoC.Extensions;

public static class SwaggerBuilderExtension
{
    public static OpenApiInfo GetOpenApiInfo(this ApiVersionDescription apiVersionDescription)
    {
        return new OpenApiInfo
        {
            Title = "WeatherAPI",
            Description = "TODO > Describe",
            Version = apiVersionDescription.GroupName + "_API",
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
        };
    }
}