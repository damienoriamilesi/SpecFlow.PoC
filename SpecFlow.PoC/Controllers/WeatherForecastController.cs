using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SpecFlow.PoC.Features;
using SpecFlow.PoC.Features.UpdateWeather;
using Swashbuckle.AspNetCore.Filters;
#pragma warning disable CS1591

namespace SpecFlow.PoC.Controllers;

/// <summary>
/// Weather forecast
/// </summary>
[ApiController]
[ApiVersion(1)]
[ApiVersion(2)]

[Route("api/v{version:apiVersion}/[controller]s")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMediator _mediator;
    private readonly ApplicationDbContext _context;
    private readonly IDataProtector _dataProtector;

    /// <summary>
    /// WeatherForecastController
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mediator"></param>
    /// <param name="dataProtectionProvider"></param>
    /// <param name="context"></param>
    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator, 
                                    IDataProtectionProvider dataProtectionProvider, ApplicationDbContext context)
    {
        _logger = logger;
        _mediator = mediator;
        _context = context;
        _dataProtector = dataProtectionProvider.CreateProtector("WeatherAPI");
    }

    /// <summary>
    /// JUST DO IT
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "{apiVersion:Version}/GetWeatherForecast")]
    [ProducesResponseType(typeof(string[]),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [MapToApiVersion(1)]
    public IEnumerable<WeatherForecast> Get()
    {
        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary =  _dataProtector.Protect(Summaries[Random.Shared.Next(Summaries.Length)])
        })
        .ToArray();

        return result;
    }
    
    /// <summary>
    /// JUST DO IT with a parameter
    /// </summary>
    /// <example>42</example>
    /// <returns></returns>
    [HttpGet("{id:int}" ,Name = "GetWeatherForecastWithParam")]
    [ProducesResponseType(typeof(string[]),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<WeatherForecast> GetById(int id)
    {
        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary =  _dataProtector.Protect(Summaries[Random.Shared.Next(Summaries.Length)])
        })
        .First();

        return Ok(result);
    }

    /// <summary>
    /// PATCH /UpdatePartial
    /// </summary>
    /// <param name="patchDoc"></param>
    /// <example>application/json:[{"operationType": 0, "path": "/description",    "op": "string",    "from": "string",    "value": "TEST"}]</example>
    /// <returns></returns>
    [HttpPatch]
    public async Task<IActionResult> UpdatePartial([FromBody] JsonPatchDocument<WeatherForecast> patchDoc)
    {
        var objectToApplyTo = new WeatherForecast();
        patchDoc.ApplyTo(objectToApplyTo);

        var result = await _mediator.Send(new UpdateWeatherForecastCommand{ WeatherForecast = objectToApplyTo });
        
        await _mediator.Publish(new WeatherUpdatedNotification(new WeatherForecast{ TemperatureC = result }));
        
        return new ObjectResult(objectToApplyTo);
    }

    /// <summary>
    /// Add new weather forecast
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [SwaggerRequestExample(typeof(CreateWeatherForecastRequest),typeof(CreateWeatherForecastRequestExample))]
    [HttpPost]
    public IActionResult Create([FromBody] CreateWeatherForecastRequest request)
    {
        return CreatedAtAction(nameof(Get), request, Guid.NewGuid());
    }
    
    [HttpGet("test42/{toto}",Name = "GetById42")]
    public IActionResult GetById42(string toto)
    {
        return Ok(42);
    }
    
    [HttpGet("test666/{toto}",Name = "GetById666")]
    public IActionResult GetById666(string toto)
    {
        return Ok(666);
    }
}

public record CreateWeatherForecastRequest(Forecast[] Forecasts);

public record Forecast(string WeatherType, int Temperature);

public class DataProtectionActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Do something before the action executes.
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Do something after the action executes.
        // For each property decorated with DataProtection
        var instanceFromResponse = context.Result!.GetType().GetProperties();
    }
}

public class CreateWeatherForecastRequestExample : IExamplesProvider<CreateWeatherForecastRequest>
{
    public CreateWeatherForecastRequest GetExamples()
    {
        return new CreateWeatherForecastRequest(new []{new Forecast("Sunny", 42)});
    }
}