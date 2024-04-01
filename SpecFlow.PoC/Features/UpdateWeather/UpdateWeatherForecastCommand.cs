using MediatR;

namespace SpecFlow.PoC.Features.UpdateWeather;

public class UpdateWeatherForecastCommand : IRequest<int>
{
    public WeatherForecast WeatherForecast { get; set; }
}
public record WeatherUpdatedNotification(WeatherForecast WeatherForecast) : INotification;


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

public class UpdateWeatherForecastCommandHandler : IRequestHandler<UpdateWeatherForecastCommand, int>
{
    public Task<int> Handle(UpdateWeatherForecastCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handler");
        return Task.FromResult(request.WeatherForecast.TemperatureC);
    }
}

