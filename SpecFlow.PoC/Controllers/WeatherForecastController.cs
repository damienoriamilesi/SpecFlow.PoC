using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SpecFlow.PoC.Features;
using SpecFlow.PoC.Features.UpdateWeather;
using SpecFlow.PoC.Meters;
using Swashbuckle.AspNetCore.Filters;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

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
    private readonly ApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly EntryMeter _meter;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IDataProtector _dataProtector;

    /// <summary>
    /// WeatherForecastController
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mediator"></param>
    /// <param name="dataProtectionProvider"></param>
    /// <param name="context"></param>
    /// <param name="meter"></param>
    /// <param name="linkGenerator">Helps create HATEOAS links</param>
    /// <param name="httpContext">To generate HATEOAS links, we need HttpContext</param>
    public WeatherForecastController(ILogger<WeatherForecastController> logger, 
                                    IMediator mediator, 
                                    IDataProtectionProvider dataProtectionProvider, 
                                    ApplicationDbContext context,
                                    EntryMeter meter,
                                    LinkGenerator linkGenerator,
                                    IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _mediator = mediator;
        _context = context;
        _meter = meter;
        _linkGenerator = linkGenerator;
        _httpContext = httpContext;
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
        
        _meter.ReadsCounter.Add(1);
        
        return result;
    }
    
    /// <summary>
    /// JUST DO IT with a parameter
    /// </summary>
    /// <example>42</example>
    /// <returns></returns>
    [HttpGet("{id:int}" ,Name = "GetWeatherForecastWithParam")]
    [ProducesResponseType(typeof(GetForecastByIdResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<GetForecastByIdResponse> GetById(int id)
    {
        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary =  _dataProtector.Protect(Summaries[Random.Shared.Next(Summaries.Length)])
        })
        .First();

        var response = new GetForecastByIdResponse(result, new List<Link>());
        response.Links.Add(
            new(_linkGenerator.GetUriByAction(_httpContext.HttpContext!, nameof(GetById), values: new { id }),
                nameof(GetById), HttpMethod.Get.ToString()));
        
        return Ok(response);
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

public record GetForecastByIdResponse(WeatherForecast WeatherForecast, List<Link> Links);

public record Link(string Href, string Rel, string Method);