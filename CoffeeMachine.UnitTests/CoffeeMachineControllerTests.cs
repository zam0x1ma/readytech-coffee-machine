using AutoFixture;
using CoffeeMachine.ActionResults;
using CoffeeMachine.Controllers;
using CoffeeMachine.Data;
using CoffeeMachine.Dtos;
using CoffeeMachine.Providers;
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
    private readonly Fixture _fixture = new Fixture();

    public CoffeeMachineControllerTests()
    {
        _sut = new CoffeeMachineController(_logger, _repository, _dateTimeProvider);
    }
    
    [Fact]
    public void BrewCoffee_ShouldReturnSuccess_WhenCoffeeMachineNotEmpty()
    {
        // Arrange
        var coffeeMachine = _fixture.Build<Models.CoffeeMachine>()
            .With(x => x.CupsLeft, 4)
            .Create();
        _dateTimeProvider.Now.Returns(new DateTime(2023, 1, 1));
        _repository.GetFirstCoffeeMachine().Returns(coffeeMachine);

        // Act
        var result = _sut.BrewCoffee();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull()
            .And.BeOfType<OkObjectResult>();
        ((OkObjectResult)result.Result).Value.Should().NotBeNull()
            .And.BeOfType<CoffeeMachineResponseDto>();
        _repository.Received(1).GetFirstCoffeeMachine();
        _repository.Received(1).SaveChanges();
    }

    [Fact]
    public void BrewCoffee_ShouldReturnServiceUnavailable_WhenCoffeeMachineEmpty()
    {
        // Arrange
        var coffeeMachine = _fixture.Build<Models.CoffeeMachine>()
            .With(x => x.CupsLeft, 0)
            .Create();
        _dateTimeProvider.Now.Returns(new DateTime(2023, 1, 1));
        _repository.GetFirstCoffeeMachine().Returns(coffeeMachine);

        // Act
        var result = _sut.BrewCoffee();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull()
            .And.BeOfType<StatusCodeWithEmptyBodyResult>();
        ((StatusCodeWithEmptyBodyResult)result.Result).StatusCode.Should().NotBeNull()
            .And.Be(StatusCodes.Status503ServiceUnavailable);
        _repository.Received(1).GetFirstCoffeeMachine();
        _repository.Received(1).SaveChanges();
    }

    [Fact]
    public void BrewCoffee_ShouldReturnImATeapot_WhenIs1stApril()
    {
        // Arrange
        var coffeeMachine = _fixture.Create<Models.CoffeeMachine>();
        _dateTimeProvider.Now.Returns(new DateTime(2023, 4, 1));
        _repository.GetFirstCoffeeMachine().Returns(coffeeMachine);

        // Act
        var result = _sut.BrewCoffee();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().NotBeNull()
            .And.BeOfType<StatusCodeWithEmptyBodyResult>();
        ((StatusCodeWithEmptyBodyResult)result.Result).StatusCode.Should().NotBeNull()
            .And.Be(StatusCodes.Status418ImATeapot);
        _repository.DidNotReceive().GetFirstCoffeeMachine();
        _repository.DidNotReceive().SaveChanges(); 
    }
}