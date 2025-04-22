using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpecFlow.PoC.Controllers;

namespace SpecFlow.PoC.Extensions.OpenApiSpec;

#pragma warning disable CS1591
public class ConfigureControllersOptions : IConfigureNamedOptions<MvcOptions>
{
    public void Configure(MvcOptions options)
    {
        options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
        options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
        options.Filters.Add<DataProtectionActionFilter>();
        // Required for Content Negotiation 
        options.ReturnHttpNotAcceptable = true;
    }

    public void Configure(string name, MvcOptions options)
    {
        Configure(options);
    }
}

public class ConfigureProblemDetailsOptions : IConfigureNamedOptions<ProblemDetailsOptions>
{
    public void Configure(ProblemDetailsOptions options)
    {
        
    }

    public void Configure(string name, ProblemDetailsOptions options)
    {
        Configure(options);
    }
}