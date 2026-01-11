namespace MinimumAPI.Domain;

/// <summary>
/// 予報データのモデル（ドメイン）です。
/// </summary>
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    // 華氏に換算した派生プロパティ
    /// <summary>
    /// 温度を華氏に換算した派生プロパティです。
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
