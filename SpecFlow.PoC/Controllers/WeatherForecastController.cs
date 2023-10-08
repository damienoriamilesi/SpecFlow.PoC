using System.Net;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace SpecFlow.PoC.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMediator _mediator;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [ProducesResponseType(typeof(string[]),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPatch]
    public async Task<IActionResult> UpdatePartial([FromBody] JsonPatchDocument<WeatherForecast> patchDoc)
    {
        var objectToApplyTo = new WeatherForecast();
        patchDoc.ApplyTo(objectToApplyTo);

        var result = await _mediator.Send(new UpdateWeatherForecastRequest{ WeatherForecast = objectToApplyTo });
        
        await _mediator.Publish(new WeatherUpdatedNotification(new WeatherForecast{ TemperatureC = result }));
        
        return new ObjectResult(objectToApplyTo);
    }
}

public class WeatherUpdatedNotificationHandler : INotificationHandler<WeatherUpdatedNotification>
{
    public Task Handle(WeatherUpdatedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"WeatherUpdated : { notification.WeatherForecast?.TemperatureC }");
        return Task.CompletedTask;
    }
}
public class WeatherUpdatedAgainNotificationHandler : INotificationHandler<WeatherUpdatedNotification>
{
    public Task Handle(WeatherUpdatedNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"WeatherUpdated again : { notification.WeatherForecast?.TemperatureF }");
        return Task.CompletedTask;
    }
}

public class UpdateWeatherForecastRequestHandler : IRequestHandler<UpdateWeatherForecastRequest, int>
{
    public Task<int> Handle(UpdateWeatherForecastRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handler");
        return Task.FromResult(request.WeatherForecast.TemperatureC);
    }
}

public record WeatherUpdatedNotification(WeatherForecast WeatherForecast) : INotification;

public class UpdateWeatherForecastRequest : IRequest<int>
{
    public WeatherForecast WeatherForecast { get; set; }
}

