using Microsoft.Extensions.DependencyInjection;
using MinimumAPI.Application.Abstractions;
using MinimumAPI.Infrastructure;
using MinimumAPI.Infrastructure.WeatherForecast;
using Xunit;

namespace MinimumAPI.Tests.Infrastructure;

public class DependencyInjectionTests
{
    [Fact(DisplayName = "Infrastructure登録の場合、天気予報クエリが解決できること")]
    public void AddInfrastructure_RegistersWeatherForecastQuery()
    {
        // Arrange: DI コンテナを用意
        var services = new ServiceCollection();

        // Act: Infrastructure 層の登録を実行
        services.AddInfrastructure();

        // Assert: 目的のサービスが解決できる
        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IWeatherForecastQuery>();

        Assert.NotNull(service);
        Assert.IsType<WeatherForecastQuery>(service);
    }
}
