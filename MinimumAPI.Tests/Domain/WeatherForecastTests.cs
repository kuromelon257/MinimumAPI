using System;
using MinimumAPI.Domain;
using Xunit;

namespace MinimumAPI.Tests.Domain;

public class WeatherForecastTests
{
    [Fact(DisplayName = "0℃の場合、華氏は32であること")]
    public void TemperatureF_ConvertsFromCelsius()
    {
        // Arrange: 0℃の予報を用意
        var forecast = new WeatherForecast(new DateOnly(2020, 1, 1), 0, "Test");

        // Assert: 華氏が32になる
        Assert.Equal(32, forecast.TemperatureF);
    }
}
