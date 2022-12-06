using System.ComponentModel.DataAnnotations;

namespace CoffeeMachine.Models;

public class CoffeeMachine
{
    [Key]
    public int Id { get; set; }

    private readonly int _cupsCapacity = 4;

    public int CupsLeft { get; set; }

    public bool IsEmpty => CupsLeft == 0;

    public void Refill()
    {
        CupsLeft = _cupsCapacity;
    }

    public void Serve()
    {
        CupsLeft--;
    }
}
