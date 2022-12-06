using CoffeeMachine.Dtos;

namespace CoffeeMachine.Services;

public class ExternalWeatherService : IExternalWeatherService
{
    private readonly string _wellingtonLatitude = "-41.28664";
    private readonly string _wellingLongitude = "174.77557";
    private readonly string _apiKey = "8151af6d0057666f02e37aa1b1f4d91f";
    
    public async Task<decimal?> GetCurrentTemperatureInWellington()
    {
        using var httpClient = new HttpClient();
        var uri =
            $"https://api.openweathermap.org/data/2.5/weather?lat={_wellingtonLatitude}&lon={_wellingLongitude}&units=metric&appid={_apiKey}";
        var httpResponse = await httpClient.GetAsync(uri);
        httpResponse.EnsureSuccessStatusCode();

        try
        {
            var openWeatherDto = await httpResponse.Content.ReadFromJsonAsync<OpenWeatherDto>();

            return openWeatherDto?.Main.Temp;
        }
        catch
        {
            Console.WriteLine("Deserialization error.");
        }

        return null;
    }
}