using CoffeeMachine.ActionResults;
using CoffeeMachine.Data;
using CoffeeMachine.Dtos;
using CoffeeMachine.Providers;
using CoffeeMachine.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachine.Controllers;

[ApiController]
public class CoffeeMachineController : ControllerBase
{
    private readonly ILogger<CoffeeMachineController> _logger;
    private readonly ICoffeeMachineRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IExternalWeatherService _externalWeatherService;

    public CoffeeMachineController(ILogger<CoffeeMachineController> logger,
        ICoffeeMachineRepository repository,
        IDateTimeProvider dateTimeProvider,
        IExternalWeatherService externalWeatherService)
    {
        _logger = logger;
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _externalWeatherService = externalWeatherService;
    }

    [HttpGet("brew-coffee")]
    public async Task<ActionResult<CoffeeMachineResponseDto>> BrewCoffee()
    {
        var today = _dateTimeProvider.Now;

        if (today.Day == 1 && today.Month == 4)
        {
            return new StatusCodeWithEmptyBodyResult(StatusCodes.Status418ImATeapot);
        }

        var coffeeMachine = _repository.GetFirstCoffeeMachine();

        if (coffeeMachine.IsEmpty)
        {
            coffeeMachine.Refill();
            _repository.SaveChanges();

            return new StatusCodeWithEmptyBodyResult(StatusCodes.Status503ServiceUnavailable);
        }

        coffeeMachine.Serve();
        _repository.SaveChanges();

        var currentTemperatureInWellington = await _externalWeatherService.GetCurrentTemperatureInWellington();

        var response = new CoffeeMachineResponseDto
        {
            Message = currentTemperatureInWellington > 30
                ? "Your refreshing iced coffee is ready"
                : "Your piping hot coffee is ready",
            Prepared = DateTime.Now
        };

        return Ok(response);
    }
}
