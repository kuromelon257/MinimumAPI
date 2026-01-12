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

public class AutoMoqTests
{
    [Fact(DisplayName = "依存が多いクラスでも、AutoMoqで自動的にモックされること")]
    public void AutoMoq_CreatesSubjectWithMocks()
    {
        // Arrange: 依存を自動モックするFixtureを用意
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var expected = new List<WeatherForecast>
        {
            new(new DateOnly(2020, 1, 1), 10, "Test")
        };

        var queryMock = fixture.Freeze<Mock<IWeatherForecastQuery>>();
        queryMock.Setup(query => query.GetForecasts()).Returns(expected);

        // Act: 依存を注入した対象を作成して呼び出し
        var useCase = fixture.Create<WeatherForecastUseCase>();
        var result = useCase.Execute();

        // Assert: モックが返す値がそのまま返る
        Assert.Same(expected, result);
    }
}
