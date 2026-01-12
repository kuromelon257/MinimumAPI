using Microsoft.Extensions.DependencyInjection;
using MinimumAPI.Application.UseCases;

namespace MinimumAPI.Application;

/// <summary>
/// Application 層の DI 登録をまとめる拡張メソッドです。
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Application 層のサービス登録を行います。
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastUseCase, WeatherForecastUseCase>();
        return services;
    }
}
