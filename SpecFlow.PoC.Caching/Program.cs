using System.Text.Json;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
// Add Redis configuration
var redisConfiguration = builder.Configuration.GetSection("Redis")["ConnectionString"]!;
var redis = ConnectionMultiplexer.Connect(redisConfiguration);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

var configuration = ConfigurationOptions.Parse("localhost:6379");
var redisConnection = ConnectionMultiplexer.Connect(configuration);

        
/*
 IDatabase redisCache = redisConnection.GetDatabase();
string cachedData = redisCache.StringGet("cachedData")!;
if (string.IsNullOrEmpty(cachedData))
{
    cachedData = "Toto42";//GetDataFromDatabase();
    redisCache.StringSet("cachedData", cachedData, TimeSpan.FromSeconds(10));
}
*/
var app = builder.Build();

app.MapGet("/test",  GetDataHandler);

Func<Task<string>> GetDataHandler(IRedisCacheService cache)
{
    return async () => await cache.GetCacheValueAsync<string>("cachedData")!;
}

app.Run();


public interface IRedisCacheService
{
    Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration);
    Task<T> GetCacheValueAsync<T>(string key);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration)
    {
        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, json, expiration);
    }

    public async Task<T> GetCacheValueAsync<T>(string key)
    {
        var db = _redis.GetDatabase();
        var json = await db.StringGetAsync(key);
        return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;
    }
}