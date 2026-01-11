namespace MinimumAPI.Domain;

// 予報データのモデル（ドメイン）
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    // 華氏に換算した派生プロパティ
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
