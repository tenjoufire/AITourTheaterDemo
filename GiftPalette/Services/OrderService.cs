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

    public OrderService()
    {
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        // �T���v�������f�[�^�������� - InventoryService�̎��ۂ̏��i�Ɋ�Â�
        var sampleOrders = new List<Order>
        {
            new Order
            {
                Id = _nextOrderId++,
                OrderNumber = $"GP{DateTime.Now.AddDays(-10):yyyyMMdd}0001",
                CustomerName = "�c�� �Ԏq",
                CustomerEmail = "hanako.tanaka@example.com",
                ShippingAddress = "��150-0001\n�����s�a�J��_�{�O1-1-1\n�A�p�[�g�����g101",
                TotalAmount = 8380,
                Status = OrderStatus.Delivered,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                CompletedAt = DateTime.UtcNow.AddDays(-7),
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 3,
                        ProductName = "�A���}�L�����h���Z�b�g",
                        Price = 3500,
                        Quantity = 1,
                        ImageUrl = "/images/003.png"
                    },
                    new OrderItem
                    {
                        ProductId = 4,
                        ProductName = "�ԕ��n���h�N���[���Z�b�g",
                        Price = 2400,
                        Quantity = 2,
                        ImageUrl = "/images/004.png"
                    }
                }
            },
            new Order
            {
                Id = _nextOrderId++,
                OrderNumber = $"GP{DateTime.Now.AddDays(-5):yyyyMMdd}0002",
                CustomerName = "���� ���Y",
                CustomerEmail = "taro.sato@example.com",
                ShippingAddress = "��530-0001\n���{���s�k��~�c1-1-1\n�}���V����202",
                TotalAmount = 17000,
                Status = OrderStatus.Shipped,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 10,
                        ProductName = "スマートウォッチ",
                        Price = 15800,
                        Quantity = 1,
                        ImageUrl = "/images/010.png"
                    },
                    new OrderItem
                    {
                        ProductId = 34,
                        ProductName = "�X�}�[�g�t�H���X�^���h",
                        Price = 1800,
                        Quantity = 1,
                        ImageUrl = "/images/034.png"
                    }
                }
            },
            new Order
            {
                Id = _nextOrderId++,
                OrderNumber = $"GP{DateTime.Now.AddDays(-2):yyyyMMdd}0003",
                CustomerName = "�R�c ����",
                CustomerEmail = "misaki.yamada@example.com",
                ShippingAddress = "��220-0001\n�_�ސ쌧���l�s����݂ȂƂ݂炢1-1-1\n�^���[1001",
                TotalAmount = 8800,
                Status = OrderStatus.Processing,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 24,
                        ProductName = "�A���}�����܃Z�b�g",
                        Price = 2400,
                        Quantity = 1,
                        ImageUrl = "/images/024.png"
                    },
                    new OrderItem
                    {
                        ProductId = 41,
                        ProductName = "�o�X���[�u",
                        Price = 6200,
                        Quantity = 1,
                        ImageUrl = "/images/041.png"
                    },
                    new OrderItem
                    {
                        ProductId = 6,
                        ProductName = "���j�R�[���X���b�p",
                        Price = 2200,
                        Quantity = 1,
                        ImageUrl = "/images/006.png"
                    }
                }
            },
            new Order
            {
                Id = _nextOrderId++,
                OrderNumber = $"GP{DateTime.Now.AddDays(-1):yyyyMMdd}0004",
                CustomerName = "��� ����",
                CustomerEmail = "kenichi.suzuki@example.com",
                ShippingAddress = "��450-0001\n���m�����É��s�����於�w1-1-1\n�r��501",
                TotalAmount = 17300,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = 9,
                        ProductName = "���U�[�r�W�l�X�蒠",
                        Price = 8500,
                        Quantity = 1,
                        ImageUrl = "/images/009.png"
                    },
                    new OrderItem
                    {
                        ProductId = 21,
                        ProductName = "�����Y�V���o�[�l�b�N���X",
                        Price = 9800,
                        Quantity = 1,
                        ImageUrl = "/images/021.png"
                    }
                }
            }
        };

        _orders.AddRange(sampleOrders);
    }

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