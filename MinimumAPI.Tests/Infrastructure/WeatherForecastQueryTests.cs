using System;
using MinimumAPI.Infrastructure.WeatherForecast;
using Xunit;

namespace MinimumAPI.Tests.Infrastructure;

public class WeatherForecastQueryTests
{
    [Fact(DisplayName = "予報を取得した場合、5件の要素が範囲内で返ること")]
    public void GetForecasts_ReturnsFiveItemsWithinExpectedRange()
    {
        // Arrange: クエリの実装を用意
        var query = new WeatherForecastQuery();

        // Act: 予報を取得
        var forecasts = query.GetForecasts();

        // Assert: 件数と各要素の範囲を確認
        Assert.Equal(5, forecasts.Count);

        var today = DateOnly.FromDateTime(DateTime.Now);
        foreach (var forecast in forecasts)
        {
            Assert.InRange(forecast.Date, today.AddDays(1), today.AddDays(5));
            Assert.InRange(forecast.TemperatureC, -20, 55);
            Assert.False(string.IsNullOrWhiteSpace(forecast.Summary));
        }
    }
}
