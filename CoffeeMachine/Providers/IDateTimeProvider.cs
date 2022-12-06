namespace CoffeeMachine.Providers;

public interface IDateTimeProvider
{
    public DateTime Now { get; }
}
