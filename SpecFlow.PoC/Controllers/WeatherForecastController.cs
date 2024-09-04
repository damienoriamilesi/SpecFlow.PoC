using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SpecFlow.PoC.Features;
using SpecFlow.PoC.Features.UpdateWeather;
#pragma warning disable CS1591

namespace SpecFlow.PoC.Controllers;

/// <summary>
/// Weather forecast
/// </summary>
[ApiController]
//[Produces("application/json")]
[Consumes("application/json")]
[Route("api/[controller]s")]
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
    [HttpGet(Name = "GetWeatherForecast")]
    [ProducesResponseType(typeof(string[]),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    /// <example>{"forecasts": [{"weatherType": "Sunny","temperature": 15}]}</example>
    /// <returns></returns>
    [HttpPost]
    //[DefaultValue(typeof(CreateWeatherForecastRequest), "{ 'Forecasts': [{ 'WeatherType' : 'Sunny', 'Temperature': 25 }]")]
    
    public IActionResult Create([FromBody] CreateWeatherForecastRequest request)
    {
        return CreatedAtAction(nameof(Get), request, Guid.NewGuid());
    }
}

public record CreateWeatherForecastRequest(Forecast[] Forecasts);

public record Forecast(string WeatherType, int Temperature);

public class DatProtectionActionFilter : IActionFilter
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