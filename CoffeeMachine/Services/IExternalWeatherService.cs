namespace CoffeeMachine.Services;

public interface IExternalWeatherService
{
    Task<decimal?> GetCurrentTemperatureInWellington();
}