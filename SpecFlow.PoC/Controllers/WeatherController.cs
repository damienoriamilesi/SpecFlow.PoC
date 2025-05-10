using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecFlow.PoC.Features;
#pragma warning disable CS1591

namespace SpecFlow.PoC.Controllers;

/// <summary>
/// Weather forecast
/// </summary>
[ApiController]
[ApiVersion(1)]
[ApiVersion(2)]

[Route("api/v{version:apiVersion}/[controller]s")]
public class WeatherController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMediator _mediator;

    /// <summary>
    /// WeatherController
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="context"></param>
    public WeatherController(IMediator mediator,
        ApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    /// <summary>
    /// JUST DO IT
    /// </summary>
    /// <returns></returns>
    [HttpGet( "", Name = "GetWeather" )]
    [ProducesResponseType(typeof(WeatherForecast[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [MapToApiVersion(1)]
    public async Task<WeatherForecast[]> Get()
    {
        var result = await _mediator.Send(new GetWeather.Request(42));
        return result;
    }
    
    /// <summary>
    /// JUST DO IT
    /// </summary>
    /// <returns></returns>
    [HttpGet("search", Name = "GetWeatherByFilter")]
    [ProducesResponseType(typeof(WeatherForecast[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [MapToApiVersion(1)]
    public async Task<WeatherForecast[]> Get([FromQuery] GetWeatherRequest request)
    {
        var result = await _mediator.Send(new GetWeather.Request(42));
        return result;
    }
}

public class GetWeatherRequest
{
    [Required]
    public string Label { get; set; }

    [Required]
    [Range(1, 8)]
    public int Code { get; set; }
}

public static class GetWeather
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    public record Request(int RandomParam) : IRequest<WeatherForecast[]>;

    public class GetWeatherRequestHandler : IRequestHandler<Request, WeatherForecast[]>
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public GetWeatherRequestHandler(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<WeatherForecast[]> Handle(Request request, CancellationToken cancellationToken)
        {
            var employee = await _applicationDbContext.Employees.FirstAsync(cancellationToken: cancellationToken);
            
            var result = Enumerable.Range(1, request.RandomParam).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                    Employee = employee
                })
                .ToArray();
            return await Task.FromResult(result);
        }
    }
} 