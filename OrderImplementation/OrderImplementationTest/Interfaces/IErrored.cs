using OrderConsoleApp.Models;

namespace OrderConsoleApp.Interfaces;
public interface IErrored
{
    event ErroredEventHandler ErroredHandler;
}

