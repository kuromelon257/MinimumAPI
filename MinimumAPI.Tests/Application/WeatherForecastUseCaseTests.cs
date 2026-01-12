using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using MinimumAPI.Application.Abstractions;
using MinimumAPI.Application.UseCases;
using MinimumAPI.Domain;
using Xunit;

namespace MinimumAPI.Tests.Application;

public class WeatherForecastUseCaseTests
{
    [Fact(DisplayName = "予報取得のユースケースを実行した場合、クエリが1回呼ばれること")]
    public void Execute_CallsQueryOnce()
    {
        // Arrange: 依存を自動モックするFixtureを用意
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var expected = new List<WeatherForecast>
        {
            new(new DateOnly(2020, 1, 1), 10, "Test")
        };
        var queryMock = fixture.Freeze<Mock<IWeatherForecastQuery>>();
        queryMock.Setup(query => query.GetForecasts()).Returns(expected);

        var useCase = fixture.Create<WeatherForecastUseCase>();

        // Act
        var result = useCase.Execute();

        // Assert
        Assert.Same(expected, result);
        queryMock.Verify(query => query.GetForecasts(), Times.Once);
    }
}
