namespace GiftPalette.API.Models;

public class CartItem
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class Cart
{
    public string CartId { get; set; } = Guid.NewGuid().ToString();
    public List<CartItem> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
    public int TotalItems => Items.Sum(item => item.Quantity);
}