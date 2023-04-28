using OrderConsoleApp.Interfaces;

namespace OrderConsoleApp.Models;

public class FilledOrderState : IOrderState
{
    public void RespondToTick(string? code, int? quantity, decimal? price, decimal? threshold)
    {
        Console.WriteLine($"Order of {code} already filled");
    }
}

