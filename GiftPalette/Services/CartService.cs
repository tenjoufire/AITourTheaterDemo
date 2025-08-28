using GiftPalette.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

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
    
    public string CartId { get; }

    public CartService(HttpClient httpClient, IOptions<ApiConfiguration> apiConfig)
    {
        _httpClient = httpClient;
        _baseUrl = apiConfig.Value.BaseUrl;
        CartId = GetOrCreateCartId();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private static string GetOrCreateCartId()
    {
        // In a real app, this would be stored in browser storage or session
        return "user-cart-" + Guid.NewGuid().ToString()[..8];
    }

    public async Task<Cart> GetCartAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/cart/{CartId}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var cart = JsonSerializer.Deserialize<Cart>(json, _jsonOptions);
                return cart ?? new Cart { CartId = CartId };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching cart: {ex.Message}");
        }
        
        return new Cart { CartId = CartId };
    }

    public async Task<Cart> AddToCartAsync(int productId, int quantity)
    {
        try
        {
            var request = new AddToCartRequest(productId, quantity);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/cart/{CartId}/add", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var cart = JsonSerializer.Deserialize<Cart>(responseJson, _jsonOptions);
                return cart ?? new Cart { CartId = CartId };
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
        try
        {
            var request = new UpdateCartRequest(productId, quantity);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{_baseUrl}/api/cart/{CartId}/update", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var cart = JsonSerializer.Deserialize<Cart>(responseJson, _jsonOptions);
                return cart ?? new Cart { CartId = CartId };
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
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/cart/{CartId}/remove/{productId}");
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var cart = JsonSerializer.Deserialize<Cart>(responseJson, _jsonOptions);
                return cart ?? new Cart { CartId = CartId };
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
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/cart/{CartId}/clear");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing cart: {ex.Message}");
            return false;
        }
    }
}