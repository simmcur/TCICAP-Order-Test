using OrderConsoleApp.Interfaces;

namespace OrderConsoleApp.Models;

public class ErroredOrderState : IOrderState
{
    public void RespondToTick(string? code, int? quantity, decimal? price, decimal? threshold)
    {
        Console.WriteLine($"Order of {code} errored at {price}");
    }
}