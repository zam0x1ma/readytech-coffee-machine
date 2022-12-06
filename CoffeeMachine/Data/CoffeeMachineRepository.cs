namespace CoffeeMachine.Data;

public class CoffeeMachineRepository : ICoffeeMachineRepository
{
    private readonly AppDbContext _context;

    public CoffeeMachineRepository(AppDbContext context)
    {
        _context = context;

        PopulateDb();
    }

    public void CreateCoffeMachine(Models.CoffeeMachine coffeeMachine)
    {
        if (coffeeMachine is null)
        {
            throw new ArgumentNullException(nameof(coffeeMachine));
        }

        _context.CoffeeMachines.Add(coffeeMachine);
    }

    public Models.CoffeeMachine GetFirstCoffeeMachine()
    {
        return _context.CoffeeMachines.FirstOrDefault();
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }

    private void PopulateDb()
    {
        _context.CoffeeMachines.Add(
            new Models.CoffeeMachine
            {
                CupsLeft = 4
            }
        );
        _context.SaveChanges();
    }
}
