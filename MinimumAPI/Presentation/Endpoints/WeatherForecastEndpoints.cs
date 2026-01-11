using MinimumAPI.Domain;

namespace MinimumAPI.Presentation.Endpoints;

// 天気予報関連のエンドポイント定義
public static class WeatherForecastEndpoints
{
    public static IEndpointRouteBuilder MapWeatherForecastEndpoints(this IEndpointRouteBuilder app)
    {
        // 天気のダミー文言
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        // GET /weatherforecast で 5 日分の予報を返す
        app.MapGet("/weatherforecast", () =>
        {
            // 今日から 5 日分のランダムなデータを生成
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    // 日付、気温(C)、概要をセット
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            // 生成した配列をそのまま返す（JSON 化される）
            return forecast;
        })
        // エンドポイント名を付ける（ルートの識別子）
        .WithName("GetWeatherForecast");

        return app;
    }
}
