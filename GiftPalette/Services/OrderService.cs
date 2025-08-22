using GiftPalette.Models;

namespace GiftPalette.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Cart cart, string customerName, string customerEmail, string shippingAddress);
    Task<List<Order>> GetOrderHistoryAsync();
    Task<Order?> GetOrderAsync(int orderId);
}

public class OrderService : IOrderService
{
    private readonly List<Order> _orders = new();
    private int _nextOrderId = 1;

    public Task<Order> CreateOrderAsync(Cart cart, string customerName, string customerEmail, string shippingAddress)
    {
        var order = new Order
        {
            Id = _nextOrderId++,
            OrderNumber = $"GP{DateTime.Now:yyyyMMdd}{_nextOrderId:D4}",
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            ShippingAddress = shippingAddress,
            TotalAmount = cart.TotalAmount,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = cart.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity,
                ImageUrl = item.ImageUrl
            }).ToList()
        };

        _orders.Add(order);
        return Task.FromResult(order);
    }

    public Task<List<Order>> GetOrderHistoryAsync()
    {
        return Task.FromResult(_orders.OrderByDescending(o => o.CreatedAt).ToList());
    }

    public Task<Order?> GetOrderAsync(int orderId)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        return Task.FromResult(order);
    }
}