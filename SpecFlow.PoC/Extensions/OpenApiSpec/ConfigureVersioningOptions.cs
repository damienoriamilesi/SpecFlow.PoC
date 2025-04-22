using Asp.Versioning;
using Microsoft.Extensions.Options;

namespace SpecFlow.PoC.Extensions.OpenApiSpec;

#pragma warning disable CS1591
public class ConfigureVersioningOptions : IConfigureNamedOptions<ApiVersioningOptions>
{
    public void Configure(ApiVersioningOptions options)
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    }

    public void Configure(string name, ApiVersioningOptions options)
    {
        Configure(options);
    }
}