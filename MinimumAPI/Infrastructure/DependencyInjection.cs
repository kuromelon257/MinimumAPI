using Microsoft.Extensions.DependencyInjection;
using MinimumAPI.Application.Abstractions;
using MinimumAPI.Infrastructure.WeatherForecast;

namespace MinimumAPI.Infrastructure;

/// <summary>
/// Infrastructure 層の DI 登録をまとめる拡張メソッドです。
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Infrastructure 層のサービス登録を行います。
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IWeatherForecastQuery, WeatherForecastQuery>();
        return services;
    }
}
