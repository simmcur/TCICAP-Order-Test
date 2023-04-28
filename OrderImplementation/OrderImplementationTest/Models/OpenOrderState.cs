using OrderConsoleApp.Interfaces;

namespace OrderConsoleApp.Models;

public class OpenOrderState : IOrderState
{
    private Order _order;
    private IOrderService _orderService;
    private PlacedEventHandler? _placedHandler;   

    public OpenOrderState(
        Order order,
        IOrderService orderService,
        PlacedEventHandler placedHandler)
    {
        _order = order;
        _orderService = orderService;
        _placedHandler = placedHandler;       
    }

    public void RespondToTick(string? code, int? quantity, decimal? price, decimal? threshold)
    {
        if (price < threshold)
        {
            if (quantity != default && code != default && price != default)
            {
                _orderService.Buy(code, quantity.Value, price.Value);

                _placedHandler?.Invoke(new PlacedEventArgs(code, price.Value));

                _order.Change(new FilledOrderState());
            }
        }
    }
}
