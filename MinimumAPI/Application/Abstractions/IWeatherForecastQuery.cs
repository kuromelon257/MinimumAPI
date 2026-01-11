using MinimumAPI.Domain;

namespace MinimumAPI.Application.Abstractions;

/// <summary>
/// 天気予報データの取得方法を表す抽象です。
/// </summary>
public interface IWeatherForecastQuery
{
    /// <summary>
    /// 予報データ一覧を取得します。
    /// </summary>
    IReadOnlyList<WeatherForecast> GetForecasts();
}
