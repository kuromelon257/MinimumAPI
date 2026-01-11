using MinimumAPI.Application.Abstractions;

namespace MinimumAPI.Presentation.Endpoints;

/// <summary>
/// 天気予報関連のエンドポイント定義です。
/// </summary>
public static class WeatherForecastEndpoints
{
    /// <summary>
    /// 天気予報関連のエンドポイントを登録します。
    /// </summary>
    public static IEndpointRouteBuilder MapWeatherForecastEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /weatherforecast で 5 日分の予報を返す
        app.MapGet("/weatherforecast", (IWeatherForecastQuery query) => query.GetForecasts())
        // エンドポイント名を付ける（ルートの識別子）
        .WithName("GetWeatherForecast");

        return app;
    }
}
