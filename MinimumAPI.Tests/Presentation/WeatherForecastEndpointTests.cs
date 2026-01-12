using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MinimumAPI.Application;
using MinimumAPI.Application.UseCases;
using MinimumAPI.Presentation.Endpoints;
using Xunit;

namespace MinimumAPI.Tests.Presentation;

public class WeatherForecastEndpointTests
{
    [Fact(DisplayName = "エンドポイントを登録した場合、UseCaseが1回呼ばれること")]
    public async Task MapWeatherForecastEndpoints_CallsUseCaseOnce()
    {
        // Arrange: WebApplicationを構築（起動はしない）
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var useCaseMock = fixture.Freeze<Mock<IWeatherForecastUseCase>>();
        useCaseMock.Setup(useCase => useCase.Execute()).Returns([]);

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddApplication();
        builder.Services.AddSingleton<IWeatherForecastUseCase>(useCaseMock.Object);
        var app = builder.Build();

        // Act: エンドポイント登録
        app.MapWeatherForecastEndpoints();

        // Assert: エンドポイント実行でUseCaseが呼ばれる
        var endpointRouteBuilder = (IEndpointRouteBuilder)app;
        var endpoints = endpointRouteBuilder.DataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>();
        var weatherEndpoint = endpoints.First(endpoint =>
            endpoint.RoutePattern.RawText == "/weatherforecast" &&
            endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.Contains("GET") == true &&
            endpoint.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName == "GetWeatherForecast");

        var context = new DefaultHttpContext
        {
            RequestServices = app.Services,
            Response = { Body = new MemoryStream() }
        };

        Assert.NotNull(weatherEndpoint.RequestDelegate);
        await weatherEndpoint.RequestDelegate(context);
        useCaseMock.Verify(useCase => useCase.Execute(), Times.Once);
    }
}
