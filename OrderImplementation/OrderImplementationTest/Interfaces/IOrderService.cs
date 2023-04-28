namespace OrderConsoleApp.Interfaces;
public interface IOrderService
{
    void Buy(string code, int quantity, decimal price);

    void Sell(string code, int quantity, decimal price);
}

