using OrderConsoleApp.Models;

namespace OrderConsoleApp.Interfaces;

public interface IPlaced
{
    event PlacedEventHandler PlacedHandler;
}

