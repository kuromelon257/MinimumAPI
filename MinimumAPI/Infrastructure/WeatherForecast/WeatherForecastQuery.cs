using MinimumAPI.Application.Abstractions;
using DomainWeatherForecast = MinimumAPI.Domain.WeatherForecast;

namespace MinimumAPI.Infrastructure.WeatherForecast;

/// <summary>
/// ランダム生成による天気予報取得の実装です。
/// </summary>
public sealed class WeatherForecastQuery : IWeatherForecastQuery
{
    private static readonly string[] Summaries =
    [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    ];

    /// <summary>
    /// 5 日分のランダムな予報データを生成して返します。
    /// </summary>
    public IReadOnlyList<DomainWeatherForecast> GetForecasts()
    {
        return Enumerable.Range(1, 5).Select(index =>
                new DomainWeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
            .ToArray();
    }
}
