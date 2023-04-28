using OrderConsoleApp.Interfaces;

namespace OrderConsoleApp.Models;

public delegate void ErroredEventHandler(ErroredEventArgs e);
public delegate void PlacedEventHandler(PlacedEventArgs e);

public class Order : IOrder
{
    private readonly object _locker = new();
    private readonly IOrderService _orderService;
    private readonly decimal _threshold;  
    private IOrderState _state;

    public event PlacedEventHandler PlacedHandler;
    public event ErroredEventHandler ErroredHandler;

    public int Quantity { get; set; }
    public OrderStatus Status { get; set; }
    public string Code { get; set; } = null!;

    public Order(IOrderService orderService, decimal threshold)
    {
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));

        if (threshold == default)
        {
            throw new ArgumentException("Threshold value cannot be zero", nameof(threshold));
        }

        _threshold = threshold;

        PlacedHandler += Order_Placed;
        ErroredHandler += Order_Errored;

        _state = new OpenOrderState(this, _orderService, PlacedHandler);

        Status = OrderStatus.Open;
    }

    private void Order_Errored(ErroredEventArgs e)
    {
        Console.WriteLine($"Order of {e.Code} errored at {e.Price}");

        Status = OrderStatus.Faulted;
    }

    private void Order_Placed(PlacedEventArgs e)
    {
        Console.WriteLine($"Order of {e.Code} filled at {e.Price}");

        Status = OrderStatus.Filled;
    }

    public void ProcessTick(Tick tick)
    {
        lock (_locker)
        {
            try
            {
                if (Quantity != default && Code != default && Code == tick.Code)
                {
                    RespondToTick(tick.Code, tick.Price);
                }
            }
            catch (Exception ex)
            {
                ErroredHandler?.Invoke(new ErroredEventArgs(tick.Code, tick.Price, ex));

                Change(new ErroredOrderState());
            }
        }
    }

    public void RespondToTick(string code, decimal price)
    {
        _state.RespondToTick(code, Quantity, price, _threshold);
    }

    public void Change(IOrderState orderState)
    {
        _state = orderState;
    }
}

