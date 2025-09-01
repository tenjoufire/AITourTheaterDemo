using GiftPalette.Models;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace GiftPalette.Services;

public interface ICartService
{
    Task<Cart> GetCartAsync();
    Task<Cart> AddToCartAsync(int productId, int quantity);
    Task<Cart> UpdateCartItemAsync(int productId, int quantity);
    Task<Cart> RemoveFromCartAsync(int productId);
    Task<bool> ClearCartAsync();
    string CartId { get; }
}

public class CartService : ICartService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _baseUrl;
    
    private string? _cartId; // issued by API
    public string CartId => _cartId ?? string.Empty;

    public CartService(HttpClient httpClient, IOptions<ApiConfiguration> apiConfig)
    {
        _httpClient = httpClient;
        _baseUrl = apiConfig.Value.BaseUrl;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task EnsureCartIdAsync()
    {
        if (!string.IsNullOrWhiteSpace(_cartId)) return;
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/cart/issue");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("cartId", out var idProp) ||
                    doc.RootElement.TryGetProperty("CartId", out idProp))
                {
                    _cartId = idProp.GetString();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error issuing cart id: {ex.Message}");
        }

        // Fallback to the demo fixed ID if issuing failed for any reason
        _cartId ??= "demo-cart-001";
    }

    public async Task<Cart> GetCartAsync()
    {
        await EnsureCartIdAsync();
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/cart/{_cartId}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var cart = JsonSerializer.Deserialize<Cart>(json, _jsonOptions);
                return cart ?? new Cart { CartId = _cartId! };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching cart: {ex.Message}");
        }
        
        return new Cart { CartId = _cartId! };
    }

    public async Task<Cart> AddToCartAsync(int productId, int quantity)
    {
        await EnsureCartIdAsync();
        try
        {
            var request = new AddToCartRequest(productId, quantity);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/cart/{_cartId}/add", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var cart = JsonSerializer.Deserialize<Cart>(responseJson, _jsonOptions);
                return cart ?? new Cart { CartId = _cartId! };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding to cart: {ex.Message}");
        }
        
        return await GetCartAsync();
    }

    public async Task<Cart> UpdateCartItemAsync(int productId, int quantity)
    {
        await EnsureCartIdAsync();
        try
        {
            var request = new UpdateCartRequest(productId, quantity);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{_baseUrl}/api/cart/{_cartId}/update", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var cart = JsonSerializer.Deserialize<Cart>(responseJson, _jsonOptions);
                return cart ?? new Cart { CartId = _cartId! };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating cart item: {ex.Message}");
        }
        
        return await GetCartAsync();
    }

    public async Task<Cart> RemoveFromCartAsync(int productId)
    {
        await EnsureCartIdAsync();
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/cart/{_cartId}/remove/{productId}");
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var cart = JsonSerializer.Deserialize<Cart>(responseJson, _jsonOptions);
                return cart ?? new Cart { CartId = _cartId! };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing from cart: {ex.Message}");
        }
        
        return await GetCartAsync();
    }

    public async Task<bool> ClearCartAsync()
    {
        await EnsureCartIdAsync();
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/cart/{_cartId}/clear");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing cart: {ex.Message}");
            return false;
        }
    }
}