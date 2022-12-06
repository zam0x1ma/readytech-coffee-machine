namespace CoffeeMachine.Data;

public interface ICoffeeMachineRepository
{
    bool SaveChanges();
    Models.CoffeeMachine GetFirstCoffeeMachine();
    void CreateCoffeMachine(Models.CoffeeMachine coffeeMachine);
}
