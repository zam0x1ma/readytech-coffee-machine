using CoffeeMachine.Data;
using CoffeeMachine.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachine.Controllers;

[ApiController]
public class CoffeeMachineController : ControllerBase
{
    private readonly ILogger<CoffeeMachineController> _logger;
    private readonly ICoffeeMachineRepository _repository;

    public CoffeeMachineController(ILogger<CoffeeMachineController> logger,
        ICoffeeMachineRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet("brew-coffee")]
    public ActionResult<CoffeeMachineResponseDto> BrewCoffee()
    {
        var coffeeMachine = _repository.GetFirstCoffeeMachine();

        if (coffeeMachine.IsEmpty)
        {
            coffeeMachine.Refill();
            _repository.SaveChanges();

            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
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
