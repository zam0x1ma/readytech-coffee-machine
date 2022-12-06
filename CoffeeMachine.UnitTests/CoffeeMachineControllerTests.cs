using AutoFixture;
using CoffeeMachine.ActionResults;
using CoffeeMachine.Controllers;
using CoffeeMachine.Data;
using CoffeeMachine.Dtos;
using CoffeeMachine.Providers;
using CoffeeMachine.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CoffeeMachine.UnitTests;

public class CoffeeMachineControllerTests
{
    private readonly CoffeeMachineController _sut;
    private readonly ILogger<CoffeeMachineController> _logger = Substitute.For<ILogger<CoffeeMachineController>>();
    private readonly ICoffeeMachineRepository _repository = Substitute.For<ICoffeeMachineRepository>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IExternalWeatherService _externalWeatherService = Substitute.For<IExternalWeatherService>();
    private readonly Fixture _fixture = new Fixture();

    public CoffeeMachineControllerTests()
    {
        _sut = new CoffeeMachineController(_logger, _repository, _dateTimeProvider, _externalWeatherService);
    }
    
    [Fact]
    public async Task BrewCoffee_ShouldReturnSuccess_WhenCoffeeMachineNotEmpty()
    {
        // Arrange
        var coffeeMachine = _fixture.Build<Models.CoffeeMachine>()
            .With(x => x.CupsLeft, 4)
            .Create();
        var expectedMessage = "Your piping hot coffee is ready";
        _dateTimeProvider.Now.Returns(new DateTime(2023, 1, 1));
        _repository.GetFirstCoffeeMachine().Returns(coffeeMachine);
        _externalWeatherService.GetCurrentTemperatureInWellington().Returns(10);

        // Act
        var result = await _sut.BrewCoffee();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result).Value.Should().NotBeNull()
            .And.BeOfType<CoffeeMachineResponseDto>();
        ((CoffeeMachineResponseDto)((OkObjectResult)result.Result).Value).Message.Should().BeSameAs(expectedMessage);
        _repository.Received(1).GetFirstCoffeeMachine();
        _repository.Received(1).SaveChanges();
        await _externalWeatherService.Received(1).GetCurrentTemperatureInWellington();
    }
    
    [Fact]
    public async Task BrewCoffee_ShouldReturnSuccess_WhenCoffeeMachineNotEmptyAndTemperatureAbove30()
    {
        // Arrange
        var coffeeMachine = _fixture.Build<Models.CoffeeMachine>()
            .With(x => x.CupsLeft, 4)
            .Create();
        var expectedMessage = "Your refreshing iced coffee is ready";
        _dateTimeProvider.Now.Returns(new DateTime(2023, 1, 1));
        _repository.GetFirstCoffeeMachine().Returns(coffeeMachine);
        _externalWeatherService.GetCurrentTemperatureInWellington().Returns(40);

        // Act
        var result = await _sut.BrewCoffee();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result).Value.Should().NotBeNull()
            .And.BeOfType<CoffeeMachineResponseDto>();
        ((CoffeeMachineResponseDto)((OkObjectResult)result.Result).Value).Message.Should().BeSameAs(expectedMessage);
        _repository.Received(1).GetFirstCoffeeMachine();
        _repository.Received(1).SaveChanges();
        await _externalWeatherService.Received(1).GetCurrentTemperatureInWellington();
    }

    [Fact]
    public async void BrewCoffee_ShouldReturnServiceUnavailable_WhenCoffeeMachineEmpty()
    {
        // Arrange
        var coffeeMachine = _fixture.Build<Models.CoffeeMachine>()
            .With(x => x.CupsLeft, 0)
            .Create();
        _dateTimeProvider.Now.Returns(new DateTime(2023, 1, 1));
        _repository.GetFirstCoffeeMachine().Returns(coffeeMachine);

        // Act
        var result = await _sut.BrewCoffee();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull()
            .And.BeOfType<StatusCodeWithEmptyBodyResult>();
        ((StatusCodeWithEmptyBodyResult)result.Result).StatusCode.Should().NotBeNull()
            .And.Be(StatusCodes.Status503ServiceUnavailable);
        _repository.Received(1).GetFirstCoffeeMachine();
        _repository.Received(1).SaveChanges();
        await _externalWeatherService.DidNotReceive().GetCurrentTemperatureInWellington();
    }

    [Fact]
    public async void BrewCoffee_ShouldReturnImATeapot_WhenIs1stApril()
    {
        // Arrange
        var coffeeMachine = _fixture.Create<Models.CoffeeMachine>();
        _dateTimeProvider.Now.Returns(new DateTime(2023, 4, 1));
        _repository.GetFirstCoffeeMachine().Returns(coffeeMachine);

        // Act
        var result = await _sut.BrewCoffee();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull()
            .And.BeOfType<StatusCodeWithEmptyBodyResult>();
        ((StatusCodeWithEmptyBodyResult)result.Result).StatusCode.Should().NotBeNull()
            .And.Be(StatusCodes.Status418ImATeapot);
        _repository.DidNotReceive().GetFirstCoffeeMachine();
        _repository.DidNotReceive().SaveChanges();
        await _externalWeatherService.DidNotReceive().GetCurrentTemperatureInWellington();
    }
}