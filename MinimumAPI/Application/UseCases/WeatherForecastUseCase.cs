using MinimumAPI.Application.Abstractions;
using MinimumAPI.Domain;

namespace MinimumAPI.Application.UseCases;

/// <summary>
/// 天気予報を取得するユースケースの実装です。
/// </summary>
public sealed class WeatherForecastUseCase : IWeatherForecastUseCase
{
    private readonly IWeatherForecastQuery _query;

    public WeatherForecastUseCase(IWeatherForecastQuery query)
    {
        _query = query;
    }

    public IReadOnlyList<WeatherForecast> Execute() => _query.GetForecasts();
}
