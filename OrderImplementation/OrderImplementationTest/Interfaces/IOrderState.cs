namespace OrderConsoleApp.Interfaces;

public interface IOrderState
{
    void RespondToTick(string? code, int? quantity, decimal? price, decimal? threshold);
}
