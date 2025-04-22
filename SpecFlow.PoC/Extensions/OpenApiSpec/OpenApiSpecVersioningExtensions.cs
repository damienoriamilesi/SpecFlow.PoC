using Asp.Versioning.ApiExplorer;

namespace SpecFlow.PoC.Extensions.OpenApiSpec;

#pragma warning disable CS1591
public static class OpenApiSpecVersioningExtensions
{
    public static void UseOpenApiSpecVersioning(this WebApplication webApplication)
    {
        var openApiDocumentTemplate = "/docs/api/{documentName}/open-api-document.json";
        webApplication.UseSwagger(opt =>
        {
            opt.RouteTemplate = openApiDocumentTemplate;
        });
        webApplication.UseSwaggerUI(options =>
        {
            var apiVersionDescriptionProvider = webApplication.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
            
                options.SwaggerEndpoint($"/docs/api/{description.GroupName}/open-api-document.json", $"{description.GroupName.ToUpperInvariant()} - test");
            }
        });
    }
}