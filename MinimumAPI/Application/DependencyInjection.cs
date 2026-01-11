using Microsoft.Extensions.DependencyInjection;

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
        return services;
    }
}
