using GiftPalette.API.Models;

namespace GiftPalette.API.Services;

public interface ICartService
{
    Task<Cart> GetCartAsync(string cartId);
    Task<Cart> AddToCartAsync(string cartId, int productId, int quantity);
    Task<Cart> UpdateCartItemAsync(string cartId, int productId, int quantity);
    Task<Cart> RemoveFromCartAsync(string cartId, int productId);
    Task<bool> ClearCartAsync(string cartId);
}

public class CartService : ICartService
{
    private readonly Dictionary<string, Cart> _carts = new();
    private readonly IInventoryService _inventoryService;

    public CartService(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public Task<Cart> GetCartAsync(string cartId)
    {
        if (!_carts.TryGetValue(cartId, out var cart))
        {
            cart = new Cart { CartId = cartId };
            _carts[cartId] = cart;
        }
        return Task.FromResult(cart);
    }

    public async Task<Cart> AddToCartAsync(string cartId, int productId, int quantity)
    {
        var cart = await GetCartAsync(cartId);
        var product = await _inventoryService.GetProductAsync(productId);

        if (product == null || !await _inventoryService.IsInStockAsync(productId, quantity))
            return cart;

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        
        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantity;
            if (await _inventoryService.IsInStockAsync(productId, newQuantity))
            {
                existingItem.Quantity = newQuantity;
            }
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity,
                ImageUrl = product.ImageUrl
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        return cart;
    }

    public async Task<Cart> UpdateCartItemAsync(string cartId, int productId, int quantity)
    {
        var cart = await GetCartAsync(cartId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else if (await _inventoryService.IsInStockAsync(productId, quantity))
            {
                item.Quantity = quantity;
            }
            cart.UpdatedAt = DateTime.UtcNow;
        }

        return cart;
    }

    public async Task<Cart> RemoveFromCartAsync(string cartId, int productId)
    {
        var cart = await GetCartAsync(cartId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            cart.Items.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
        }

        return cart;
    }

    public async Task<bool> ClearCartAsync(string cartId)
    {
        var cart = await GetCartAsync(cartId);
        cart.Items.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
        return true;
    }
}