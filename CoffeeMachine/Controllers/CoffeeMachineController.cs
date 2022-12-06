using CoffeeMachine.ActionResults;
using CoffeeMachine.Data;
using CoffeeMachine.Dtos;
using CoffeeMachine.Providers;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachine.Controllers;

[ApiController]
public class CoffeeMachineController : ControllerBase
{
    private readonly ILogger<CoffeeMachineController> _logger;
    private readonly ICoffeeMachineRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CoffeeMachineController(ILogger<CoffeeMachineController> logger,
        ICoffeeMachineRepository repository,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    [HttpGet("brew-coffee")]
    public ActionResult<CoffeeMachineResponseDto> BrewCoffee()
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
        else
        {
            coffeeMachine.Serve();
            _repository.SaveChanges();

            var response = new CoffeeMachineResponseDto
            {
                Message = "Your piping hot coffee is ready",
                Prepared = DateTime.Now
            };

            return Ok(response);
        }
    }
}
