namespace OrderConsoleApp.Interfaces;

public interface IOrder : IPlaced, IErrored
{
    void RespondToTick(string code, decimal price);
}


