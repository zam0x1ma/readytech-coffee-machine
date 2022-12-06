namespace CoffeeMachine.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now { get; } = DateTime.Now;
}
