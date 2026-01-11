using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MinimumAPI.Domain;
using Xunit;

namespace MinimumAPI.Tests.Presentation;

public class WeatherForecastEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WeatherForecastEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "/weatherforecast を呼んだ場合、5件が返ること")]
    public async Task GetWeatherForecast_ReturnsFiveItems()
    {
        // Arrange: テスト用の HTTP クライアントを生成
        using var client = _factory.CreateClient();

        // Act: エンドポイントを呼び出し
        using var response = await client.GetAsync("/weatherforecast");

        // Assert: ステータスと件数を確認
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var forecasts = JsonSerializer.Deserialize<List<WeatherForecast>>(json, options);

        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts!.Count);
    }
}
