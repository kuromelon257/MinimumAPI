using MinimumAPI.Domain;

namespace MinimumAPI.Application.UseCases;

/// <summary>
/// 天気予報を取得するユースケースの抽象です。
/// </summary>
public interface IWeatherForecastUseCase
{
    /// <summary>
    /// 予報データ一覧を取得します。
    /// </summary>
    IReadOnlyList<WeatherForecast> Execute();
}
