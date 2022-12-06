using Microsoft.EntityFrameworkCore;

namespace CoffeeMachine.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {

    }

    public DbSet<Models.CoffeeMachine> CoffeeMachines { get; set; }
}
